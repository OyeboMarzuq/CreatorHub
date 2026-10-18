
using System.Security.Claims;
using CreatorHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CreatorHub.Web.Controllers;

public class VotingController : Controller
{
    private readonly IVotingService _votingService;

    public VotingController(IVotingService votingService)
    {
        _votingService = votingService;
    }

    // Public page: anyone can view the matchup.
    [HttpGet]
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var matchup = await _votingService.GetRandomMatchupAsync(
            cancellationToken);

        if (matchup is null)
            return View("NoCreators");

        return View(matchup);
    }

    // Only authenticated users can submit a vote.
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Vote(
        Guid firstCreatorId,
        Guid secondCreatorId,
        Guid selectedCreatorId,
        CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Challenge();
        }

        var result = await _votingService.CastVoteAsync(
            userId,
            firstCreatorId,
            secondCreatorId,
            selectedCreatorId,
            cancellationToken);

        TempData[result.Success ? "Success" : "Error"] = result.Message;

        return RedirectToAction(nameof(Index));
    }

    // Public leaderboard.
    [HttpGet]
    public async Task<IActionResult> Leaderboard(
        CancellationToken cancellationToken)
    {
        var entries = await _votingService.GetLeaderboardAsync(
            cancellationToken: cancellationToken);

        return View(entries);
    }
}
