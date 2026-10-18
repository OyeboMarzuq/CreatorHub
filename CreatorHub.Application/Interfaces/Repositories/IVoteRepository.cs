using CreatorHub.Application.DTOs.Voting;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Interfaces.Repositories;

public interface IVoteRepository
{
    Task<bool> HasUserVotedAsync(
        Guid userId,
        Guid firstCreatorId,
        Guid secondCreatorId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Vote vote);

    Task<bool> SaveChangesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(
        int take,
        CancellationToken cancellationToken = default);
}