using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Web.Controllers;

public class CreatorsController : Controller
{
    private readonly ICreatorService _creatorService;
    private readonly CreatorHubDbContext _context;
    private readonly ILogger<CreatorsController> _logger;

    public CreatorsController(
        ICreatorService creatorService,
        CreatorHubDbContext context,
        ILogger<CreatorsController> logger)
    {
        _creatorService = creatorService;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var creators = await _creatorService.GetAllAsync();
        return View(creators);
    }

    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        try
        {
            var creator = await _creatorService.GetBySlugAsync(slug);

            if (creator == null)
                return NotFound();

            if (!string.IsNullOrWhiteSpace(creator.ProfileImageUrl))
                return View(creator);

            var platformImage = await _context.PlatformProfiles
                .AsNoTracking()
                .Where(pp => pp.CreatorId == creator.Id)
                .Where(pp => pp.ProfileImageUrl != null &&
                             pp.ProfileImageUrl != "")
                .Select(pp => pp.ProfileImageUrl)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrWhiteSpace(platformImage))
            {
                creator.ProfileImageUrl = platformImage;
                return View(creator);
            }

            var usernames = await _context.PlatformProfiles
                .AsNoTracking()
                .Where(pp => pp.CreatorId == creator.Id)
                .Select(pp => pp.Username.ToLower())
                .Distinct()
                .ToListAsync();

            var platformSlugs = await _context.PlatformProfiles
                .AsNoTracking()
                .Where(pp => pp.CreatorId == creator.Id)
                .Select(pp => pp.Platform.Slug.ToLower())
                .Distinct()
                .ToListAsync();

            if (usernames.Count > 0)
            {
                var importedImage = await _context.ProfileUrls
                    .AsNoTracking()
                    .Where(p =>
                        usernames.Contains(p.Username.ToLower()) &&
                        platformSlugs.Contains(p.Source.ToLower()) &&
                        p.ImageUrl != null &&
                        p.ImageUrl != "")
                    .Select(p => p.ImageUrl)
                    .FirstOrDefaultAsync();

                if (!string.IsNullOrWhiteSpace(importedImage))
                    creator.ProfileImageUrl = importedImage;
            }

            return View(creator);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error loading creator details for slug {Slug}",
                slug);

            return StatusCode(
                500,
                "An error occurred while loading the creator profile.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        string? q,
        string? platform = "ALL",
        string? gender = null,
        int? ageMin = null,
        int? ageMax = null,
        string? hairColor = null,
        string? hairLength = null,
        string? region = null)
    {
        try
        {
            q = q?.Trim();
            platform = NormalizePlatform(platform);
            gender = gender?.Trim();
            hairColor = hairColor?.Trim();
            hairLength = hairLength?.Trim();
            region = region?.Trim();

            if (q is { Length: > 100 })
                q = q[..100];

            bool hasAdvancedFilters =
                !string.IsNullOrWhiteSpace(gender) ||
                ageMin.HasValue ||
                ageMax.HasValue ||
                !string.IsNullOrWhiteSpace(hairColor) ||
                !string.IsNullOrWhiteSpace(hairLength) ||
                !string.IsNullOrWhiteSpace(region);

            // --------------------------------------------------
            // 1. BASIC SEARCH: imported profile URLs
            // --------------------------------------------------
            if (!hasAdvancedFilters)
            {
                var profileQuery = _context.ProfileUrls
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(q))
                {
                    profileQuery = profileQuery.Where(p =>
                        p.Username.Contains(q) ||
                        p.Url.Contains(q));
                }

                if (!IsAllPlatforms(platform))
                {
                    var aliases = GetPlatformAliases(platform);

                    profileQuery = profileQuery.Where(p =>
                        aliases.Contains(p.Source.ToLower()));
                }

                var profiles = await profileQuery
                    .OrderBy(p => p.Username)
                    .Take(100)
                    .Select(p => new SearchProfileResult
                    {
                        Username = p.Username,
                        Url = p.Url,
                        Platform = p.Source,
                        SourcePage = p.SourcePage,
                        ProfileImageUrl = p.ImageUrl
                    })
                    .ToListAsync();

                var usernames = profiles
                    .Select(p => p.Username?.Trim())
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var platformImages = new List<PlatformImageResult>();

                if (usernames.Count > 0)
                {
                    var normalizedUsernames = usernames
                        .Select(u => u!.ToLowerInvariant())
                        .Distinct()
                        .ToList();

                    platformImages = await _context.PlatformProfiles
                        .AsNoTracking()
                        .Where(pp =>
                            normalizedUsernames.Contains(
                                pp.Username.ToLower()))
                        .Select(pp => new PlatformImageResult
                        {
                            Username = pp.Username,
                            PlatformSlug = pp.Platform.Slug,
                            PlatformName = pp.Platform.Name,
                            ProfileImageUrl = pp.ProfileImageUrl,
                            CreatorImageUrl = pp.Creator.ProfileImageUrl
                        })
                        .ToListAsync();
                }

                foreach (var profile in profiles)
                {
                    if (!string.IsNullOrWhiteSpace(
                            profile.ProfileImageUrl))
                    {
                        continue;
                    }

                    var matchingImages = platformImages
                        .Where(x => string.Equals(
                            x.Username?.Trim(),
                            profile.Username?.Trim(),
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    var source = NormalizePlatform(profile.Platform);

                    var samePlatformImages = matchingImages
                        .Where(x =>
                            NormalizePlatform(x.PlatformSlug) == source ||
                            NormalizePlatform(x.PlatformName) == source)
                        .ToList();

                    var candidates = samePlatformImages.Count > 0
                        ? samePlatformImages
                        : matchingImages;

                    profile.ProfileImageUrl = candidates
                        .Select(x => FirstNonEmpty(
                            x.ProfileImageUrl,
                            x.CreatorImageUrl))
                        .FirstOrDefault(url =>
                            !string.IsNullOrWhiteSpace(url));
                }

                // FIX: Use one consistent casing for image properties.
                var profileResults = profiles.Select(p => new
                {
                    username = p.Username,
                    url = p.Url,
                    platform = p.Platform,
                    sourcePage = p.SourcePage,
                    profileImageUrl = p.ProfileImageUrl,
                    imageUrl = p.ProfileImageUrl,
                    profileImage = p.ProfileImageUrl
                }).ToList();

                _logger.LogInformation(
                    "Creator search returned {Count} imported profiles for platform {Platform}.",
                    profileResults.Count,
                    platform);

                return Json(new
                {
                    success = true,
                    count = profileResults.Count,
                    results = profileResults
                });
            }

            // --------------------------------------------------
            // 2. ADVANCED SEARCH: creator records
            // --------------------------------------------------
            var creatorQuery = _context.Creators
                .AsNoTracking()
                .Include(c => c.PlatformProfiles)
                    .ThenInclude(pp => pp.Platform)
                .Include(c => c.CreatorAttributeDefinition)
                    .ThenInclude(ca => ca.AttributeValue)
                        .ThenInclude(av =>
                            av.CreatorAttributeDefinition)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(gender))
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.Gender != null &&
                    c.Gender.ToLower() == gender.ToLower());
            }

            if (ageMin.HasValue)
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.Age.HasValue &&
                    c.Age.Value >= ageMin.Value);
            }

            if (ageMax.HasValue)
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.Age.HasValue &&
                    c.Age.Value <= ageMax.Value);
            }

            if (!string.IsNullOrWhiteSpace(region))
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.Region != null &&
                    c.Region.ToLower() == region.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(hairColor))
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.CreatorAttributeDefinition.Any(ca =>
                        ca.AttributeValue.CreatorAttributeDefinition
                            .Slug.ToLower() == "hair-color" &&
                        (
                            ca.AttributeValue.Value.ToLower() ==
                                hairColor.ToLower() ||
                            ca.AttributeValue.Slug.ToLower() ==
                                hairColor.ToLower()
                        )));
            }

            if (!string.IsNullOrWhiteSpace(hairLength))
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.CreatorAttributeDefinition.Any(ca =>
                        ca.AttributeValue.CreatorAttributeDefinition
                            .Slug.ToLower() == "hair-length" &&
                        (
                            ca.AttributeValue.Value.ToLower() ==
                                hairLength.ToLower() ||
                            ca.AttributeValue.Slug.ToLower() ==
                                hairLength.ToLower()
                        )));
            }

            if (!IsAllPlatforms(platform))
            {
                var aliases = GetPlatformAliases(platform);

                creatorQuery = creatorQuery.Where(c =>
                    c.PlatformProfiles.Any(pp =>
                        aliases.Contains(pp.Platform.Slug.ToLower()) ||
                        aliases.Contains(pp.Platform.Name.ToLower())));
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                creatorQuery = creatorQuery.Where(c =>
                    c.DisplayName.Contains(q) ||
                    c.Slug.Contains(q) ||
                    c.PlatformProfiles.Any(pp =>
                        pp.Username.Contains(q)));
            }

            var creators = await creatorQuery
                .OrderBy(c => c.DisplayName)
                .Take(100)
                .ToListAsync();

            var resultAliases = GetPlatformAliases(platform);

            var creatorResults = creators
                .SelectMany(c =>
                    c.PlatformProfiles
                        .Where(pp =>
                            IsAllPlatforms(platform) ||
                            resultAliases.Contains(
                                NormalizePlatform(pp.Platform.Slug)) ||
                            resultAliases.Contains(
                                NormalizePlatform(pp.Platform.Name)))
                        .Select(pp =>
                        {
                            var imageUrl = FirstNonEmpty(
                                pp.ProfileImageUrl,
                                c.ProfileImageUrl);

                            // FIX: Do not return duplicate property names
                            // that differ only by capitalization.
                            return new
                            {
                                username = pp.Username,
                                url = pp.ProfileUrl,
                                platform = pp.Platform.Name,
                                sourcePage = (int?)null,

                                creatorId = c.Id,
                                displayName = c.DisplayName,
                                age = c.Age,
                                gender = c.Gender,
                                country = c.Country,
                                city = c.City,
                                region = c.Region,

                                profileImageUrl = imageUrl,
                                imageUrl,
                                profileImage = imageUrl
                            };
                        }))
                .Take(100)
                .ToList();

            return Json(new
            {
                success = true,
                count = creatorResults.Count,
                results = creatorResults
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Creator search failed. Query: {Query}, Platform: {Platform}",
                q,
                platform);

            return StatusCode(500, new
            {
                success = false,
                count = 0,
                results = Array.Empty<object>(),
                message = "Creator search failed. Please try again."
            });
        }
    }

    private static bool IsAllPlatforms(string? value)
    {
        return NormalizePlatform(value) == "all";
    }

    private static string NormalizePlatform(string? value)
    {
        var platform = value?
            .Trim()
            .ToLowerInvariant() ?? "all";

        var compact = platform
            .Replace("-", "")
            .Replace("_", "")
            .Replace(" ", "");

        return compact switch
        {
            "" => "all",
            "all" => "all",
            "mydirtyhobby" => "mdh",
            "mdh" => "mdh",
            "amateurstars" => "amateur",
            "amateur" => "amateur",
            "big7" => "big7",
            "frivol" => "frivol",
            _ => compact
        };
    }

    private static string[] GetPlatformAliases(string? value)
    {
        return NormalizePlatform(value) switch
        {
            "mdh" =>
            [
                "mdh",
                "mydirtyhobby",
                "my-dirty-hobby",
                "my dirty hobby"
            ],
            "amateur" =>
            [
                "amateur",
                "amateurstars",
                "amateur-stars",
                "amateur stars"
            ],
            "big7" => ["big7"],
            "frivol" => ["frivol"],
            _ => ["all"]
        };
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value =>
            !string.IsNullOrWhiteSpace(value));
    }

    private sealed class SearchProfileResult
    {
        public string Username { get; set; } = "";
        public string Url { get; set; } = "";
        public string Platform { get; set; } = "";
        public int SourcePage { get; set; }
        public string? ProfileImageUrl { get; set; }
    }

    private sealed class PlatformImageResult
    {
        public string? Username { get; set; }
        public string? PlatformSlug { get; set; }
        public string? PlatformName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CreatorImageUrl { get; set; }
    }
}