
using CreatorHub.Application.DTOs.Voting;

namespace CreatorHub.Application.Interfaces.Services;

public interface IVotingService
{
    Task<CreatorMatchupDto?> GetRandomMatchupAsync(
        CancellationToken cancellationToken = default);

    Task<(bool Success, string Message)> CastVoteAsync(
        Guid userId,
        Guid firstCreatorId,
        Guid secondCreatorId,
        Guid selectedCreatorId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(
        int take = 50,
        CancellationToken cancellationToken = default);
}
