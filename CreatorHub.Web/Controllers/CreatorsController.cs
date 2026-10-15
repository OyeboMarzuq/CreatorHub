using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Web.Controllers;

public class CreatorsController : Controller
{
    private readonly ICreatorService _creatorService;
    private readonly CreatorHubDbContext _context;

    public CreatorsController(
        ICreatorService creatorService,
        CreatorHubDbContext context)
    {
        _creatorService = creatorService;
        _context = context;
    }

    // GET: /creators
    public async Task<IActionResult> Index()
    {
        var creators = await _creatorService.GetAllAsync();
        return View(creators);
    }

    // GET: /creators/{slug}
    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            return NotFound();

        var creator = await _creatorService.GetBySlugAsync(slug);

        if (creator == null)
            return NotFound();

        return View(creator);
    }

    // GET: /Creators/Search
    //
    // Examples:
    //
    // /Creators/Search?q=DirtyDianal69&platform=FRIVOL
    //
    // /Creators/Search?gender=Woman&ageMin=20&ageMax=30
    //     &hairColor=Black&hairLength=Short&region=Asia&platform=ALL
    //
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
        q = q?.Trim();
        platform = platform?.Trim();
        gender = gender?.Trim();
        hairColor = hairColor?.Trim();
        hairLength = hairLength?.Trim();
        region = region?.Trim();

        // ---------------------------------------------------------
        // 1. Basic search only
        // ---------------------------------------------------------
        //
        // Keep the existing ProfileUrl search working exactly as
        // it did before.
        //
        bool hasAdvancedFilters =
            !string.IsNullOrWhiteSpace(gender) ||
            ageMin.HasValue ||
            ageMax.HasValue ||
            !string.IsNullOrWhiteSpace(hairColor) ||
            !string.IsNullOrWhiteSpace(hairLength) ||
            !string.IsNullOrWhiteSpace(region);


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

            if (!string.IsNullOrWhiteSpace(platform) &&
                !platform.Equals("ALL", StringComparison.OrdinalIgnoreCase))
            {
                profileQuery = profileQuery.Where(p =>
                    p.Source.ToUpper() == platform.ToUpper());
            }

            var profileResults = await profileQuery
                .OrderBy(p => p.Username)
                .Take(100)
                .Select(p => new
                {
                    username = p.Username,
                    url = p.Url,
                    platform = p.Source,
                    sourcePage = p.SourcePage
                })
                .ToListAsync();

            return Json(new
            {
                success = true,
                count = profileResults.Count,
                results = profileResults
            });
        }
            
        var creatorQuery = _context.Creators
            .AsNoTracking()
            .Include(c => c.PlatformProfiles)
                .ThenInclude(pp => pp.Platform)
            .Include(c => c.CreatorAttributeDefinition)
                .ThenInclude(ca => ca.AttributeValue)
                    .ThenInclude(av => av.CreatorAttributeDefinition)
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
                    ca.AttributeValue.CreatorAttributeDefinition.Slug
                        .ToLower() == "hair-color" &&
                    (
                        ca.AttributeValue.Value.ToLower() == hairColor.ToLower() ||
                        ca.AttributeValue.Slug.ToLower() == hairColor.ToLower()
                    )
                ));
        }

        // ---------------------------------------------------------
        // Hair Length
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(hairLength))
        {
            creatorQuery = creatorQuery.Where(c =>
                c.CreatorAttributeDefinition.Any(ca =>
                    ca.AttributeValue.CreatorAttributeDefinition.Slug
                        .ToLower() == "hair-length" &&
                    (
                        ca.AttributeValue.Value.ToLower() == hairLength.ToLower() ||
                        ca.AttributeValue.Slug.ToLower() == hairLength.ToLower()
                    )
                ));
        }

        // ---------------------------------------------------------
        // Platform
        // ---------------------------------------------------------
        //
        // ALL = every platform.
        // Otherwise match the platform Slug or Name.
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(platform) &&
            !platform.Equals("ALL", StringComparison.OrdinalIgnoreCase))
        {
            creatorQuery = creatorQuery.Where(c =>
                c.PlatformProfiles.Any(pp =>
                    pp.Platform.Slug.ToLower() == platform.ToLower() ||
                    pp.Platform.Name.ToLower() == platform.ToLower()
                ));
        }

        // ---------------------------------------------------------
        // General search
        // ---------------------------------------------------------

        if (!string.IsNullOrWhiteSpace(q))
        {
            creatorQuery = creatorQuery.Where(c =>
                c.DisplayName.Contains(q) ||
                c.Slug.Contains(q) ||
                c.PlatformProfiles.Any(pp =>
                    pp.Username.Contains(q)));
        }

        // ---------------------------------------------------------
        // Execute query
        // ---------------------------------------------------------

        var creators = await creatorQuery
            .OrderBy(c => c.DisplayName)
            .Take(100)
            .ToListAsync();

        // ---------------------------------------------------------
        // Flatten Creator -> PlatformProfile into search results
        // ---------------------------------------------------------

        var results = creators
            .SelectMany(c =>
                c.PlatformProfiles
                    .Where(pp =>
                        string.IsNullOrWhiteSpace(platform) ||
                        platform.Equals("ALL", StringComparison.OrdinalIgnoreCase) ||
                        pp.Platform.Slug.Equals(
                            platform,
                            StringComparison.OrdinalIgnoreCase) ||
                        pp.Platform.Name.Equals(
                            platform,
                            StringComparison.OrdinalIgnoreCase))
                    .Select(pp => new
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
                        profileImageUrl = c.ProfileImageUrl
                    }))
            .Take(100)
            .ToList();

        return Json(new
        {
            success = true,
            count = results.Count,
            results
        });
    }
}