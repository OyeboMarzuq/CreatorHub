
using CreatorHub.Application.DTOs.Voting;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly CreatorHubDbContext _context;

    public VoteRepository(CreatorHubDbContext context)
    {
        _context = context;
    }

    public Task<bool> HasUserVotedAsync(
        Guid userId,
        Guid firstCreatorId,
        Guid secondCreatorId,
        CancellationToken cancellationToken = default)
    {
        return _context.Votes.AnyAsync(
            vote =>
                vote.UserId == userId &&
                vote.FirstCreatorId == firstCreatorId &&
                vote.SecondCreatorId == secondCreatorId,
            cancellationToken);
    }

    public async Task AddAsync(Vote vote)
    {
        await _context.Votes.AddAsync(vote);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveChangesAsync(
    CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            // Prevent a failed vote from remaining tracked.
            foreach (var entry in _context.ChangeTracker.Entries<Vote>()
                         .Where(e => e.State == EntityState.Added))
            {
                entry.State = EntityState.Detached;
            }

            return false;
        }
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(
        int take,
        CancellationToken cancellationToken = default)
    {
        var results = await _context.Votes
            .AsNoTracking()
            .GroupBy(vote => vote.SelectedCreatorId)
            .Select(group => new
            {
                CreatorId = group.Key,
                VoteCount = group.Count()
            })
            .OrderByDescending(item => item.VoteCount)
            .ThenBy(item => item.CreatorId)
            .Take(take)
            .Join(
                _context.Creators.AsNoTracking(),
                item => item.CreatorId,
                creator => creator.Id,
                (item, creator) => new
                {
                    item.CreatorId,
                    creator.DisplayName,
                    creator.Slug,
                    creator.ProfileImageUrl,
                    item.VoteCount
                })
            .ToListAsync(cancellationToken);

        return results
            .Select((item, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                CreatorId = item.CreatorId,
                DisplayName = item.DisplayName,
                Slug = item.Slug,
                ProfileImageUrl = item.ProfileImageUrl,
                VoteCount = item.VoteCount
            })
            .ToList();
    }
}
