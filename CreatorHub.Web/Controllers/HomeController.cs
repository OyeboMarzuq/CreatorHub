using CreatorHub.Application.Auth;
using CreatorHub.Application.Interfaces.Importers;
using CreatorHub.Infrastructure.Data;
using CreatorHub.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CreatorHub.Web.Controllers
{
    public class HomeController : Controller
    {
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
            var model = new DashboardViewModel
            {
                TotalCreators = await _context.Creators.CountAsync(),
                ActiveCreators = await _context.Creators.CountAsync(c => c.IsActive),
                TotalPlatforms = await _context.Platforms.CountAsync(),
                TotalProfileUrls = await _context.ProfileUrls.CountAsync(),
                ProcessedProfileUrls = await _context.ProfileUrls.CountAsync(p => p.IsProcessed),
                TotalMembers = await _context.Users.CountAsync(u => u.Role == UserRoles.User),

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
