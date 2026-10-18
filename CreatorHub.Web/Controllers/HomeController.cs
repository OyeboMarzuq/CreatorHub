using CreatorHub.Application.Auth;
using CreatorHub.Application.Interfaces.Importers;
using CreatorHub.Infrastructure.Data;
using CreatorHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace CreatorHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private const long MaxAvatarFileBytes = 20_000_000;

        private static readonly Regex MdhIdRegex =
            new(@"/profil/(\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly ILogger<HomeController> _logger;
        private readonly CreatorHubDbContext _context;
        private readonly ICreatorImporter _creatorImporter;

        public HomeController(
    ILogger<HomeController> logger,
    CreatorHubDbContext context,
    ICreatorImporter creatorImporter)
        {
            _logger = logger;
            _context = context;
            _creatorImporter = creatorImporter;
        }

        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Dashboard()
        {
            var totalVotes = await _context.Votes.CountAsync();

            var totalMatchups = await _context.Votes
                .Select(v => new
                {
                    v.FirstCreatorId,
                    v.SecondCreatorId
                })
                .Distinct()
                .CountAsync();

            var leaderboardData = await _context.Votes
                .GroupBy(v => v.SelectedCreatorId)
                .Select(g => new
                {
                    CreatorId = g.Key,
                    VoteCount = g.Count()
                })
                .Join(
                    _context.Creators,
                    votes => votes.CreatorId,
                    creator => creator.Id,
                    (votes, creator) => new
                    {
                        creator.Id,
                        creator.DisplayName,
                        creator.Slug,
                        creator.ProfileImageUrl,
                        votes.VoteCount
                    })
                .OrderByDescending(x => x.VoteCount)
                .ThenBy(x => x.DisplayName)
                .Take(5)
                .ToListAsync();

            var model = new DashboardViewModel
            {
                TotalCreators = await _context.Creators.CountAsync(),
                ActiveCreators = await _context.Creators.CountAsync(c => c.IsActive),
                TotalPlatforms = await _context.Platforms.CountAsync(),
                TotalProfileUrls = await _context.ProfileUrls.CountAsync(),
                ProcessedProfileUrls = await _context.ProfileUrls
                    .CountAsync(p => p.IsProcessed),
                TotalMembers = await _context.Users
                    .CountAsync(u => u.Role == UserRoles.User),

                TotalVotes = totalVotes,
                TotalMatchups = totalMatchups,

                TopCreators = leaderboardData
                    .Select((creator, index) => new DashboardViewModel.LeaderboardRow
                    {
                        Rank = index + 1,
                        CreatorId = creator.Id,
                        DisplayName = creator.DisplayName,
                        Slug = creator.Slug,
                        ProfileImageUrl = creator.ProfileImageUrl,
                        VoteCount = creator.VoteCount
                    })
                    .ToList(),

                Sources = await _context.ProfileUrls
                    .GroupBy(p => p.Source)
                    .Select(g => new DashboardViewModel.SourceProgress
                    {
                        Source = g.Key,
                        Total = g.Count(),
                        Processed = g.Count(x => x.IsProcessed)
                    })
                    .ToListAsync(),

                RecentMembers = await _context.Users
                    .Where(u => u.Role == UserRoles.User)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(8)
                    .Select(u => new DashboardViewModel.MemberRow
                    {
                        Name = u.FirstName + " " + u.LastName,
                        Email = u.Email,
                        Region = u.Region,
                        Gender = u.Gender,
                        JoinedAt = u.CreatedAt
                    })
                    .ToListAsync()
            };

            return View(model);
        }



        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFrivol(
            int batchSize = 5,
            CancellationToken cancellationToken = default)
        {
            // Keep the initial test small.
            batchSize = Math.Clamp(batchSize, 1, 10);

            try
            {
                await _creatorImporter.ImportAsync(
                    batchSize,
                    cancellationToken);

                TempData["ImportMessage"] =
                    $"Import run completed for a batch of up to {batchSize} profiles. Check the database to verify the results.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Frivol creator import failed.");

                TempData["ImportMessage"] =
                    "The import failed. Check the application logs for details.";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        // POST: /Home/ImportMdhImages  (admin only)
        // Upload mdh_avatars.csv (columns: u_id;avatar). Every MDH profile URL without a
        // picture gets the avatar whose u_id appears in its /profil/<u_id>-... link.
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(MaxAvatarFileBytes)]
        public async Task<IActionResult> ImportMdhImages(IFormFile? file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ImportMessage"] = "Choose the mdh_avatars.csv file first.";
                return RedirectToAction(nameof(Dashboard));
            }

            if (file.Length > MaxAvatarFileBytes)
            {
                TempData["ImportMessage"] = "That file is too large (limit 20 MB).";
                return RedirectToAction(nameof(Dashboard));
            }

            try
            {
                // 1. Read u_id -> avatar url, accepting only http(s) links.
                var avatars = new Dictionary<string, string>();

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    await reader.ReadLineAsync(); // header

                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var parts = line.Split(';', 2);
                        if (parts.Length != 2) continue;

                        var id = parts[0].Trim();
                        var url = parts[1].Trim();

                        if (id.Length == 0) continue;
                        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) continue;
                        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) continue;

                        avatars[id] = url;
                    }
                }

                // 2. MDH profile URLs that still have no picture.
                var candidates = await _context.ProfileUrls
                    .AsNoTracking()
                    .Where(p => p.Source.ToLower() == "mdh" && p.ImageUrl == null)
                    .Select(p => new { p.Id, p.Url })
                    .ToListAsync(HttpContext.RequestAborted);

                var updates = new List<(Guid Id, string Image)>();

                foreach (var candidate in candidates)
                {
                    var match = MdhIdRegex.Match(candidate.Url ?? "");
                    if (match.Success && avatars.TryGetValue(match.Groups[1].Value, out var image))
                        updates.Add((candidate.Id, image));
                }

                // 3. Save in chunks.
                foreach (var chunk in updates.Chunk(1000))
                {
                    var ids = chunk.Select(x => x.Id).ToList();
                    var lookup = chunk.ToDictionary(x => x.Id, x => x.Image);

                    var entities = await _context.ProfileUrls
                        .Where(p => ids.Contains(p.Id))
                        .ToListAsync(HttpContext.RequestAborted);

                    foreach (var entity in entities)
                        entity.ImageUrl = lookup[entity.Id];

                    await _context.SaveChangesAsync(HttpContext.RequestAborted);
                    _context.ChangeTracker.Clear();
                }

                TempData["ImportMessage"] =
                    $"Pictures attached to {updates.Count:N0} of {candidates.Count:N0} MDH profiles " +
                    $"({avatars.Count:N0} avatars in the file).";
            }
            catch (OperationCanceledException)
            {
                TempData["ImportMessage"] = "The picture import was cancelled before it finished.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MDH picture import failed.");
                TempData["ImportMessage"] = "The picture import failed. Check the server log for details.";
            }

            return RedirectToAction(nameof(Dashboard));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
