
using CreatorHub.Application.DTOs.Voting;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Infrastructure.Services;

public class VotingService : IVotingService
{
    private readonly ICreatorRepository _creatorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVoteRepository _voteRepository;

    public VotingService(
        ICreatorRepository creatorRepository,
        IUserRepository userRepository,
        IVoteRepository voteRepository)
    {
        _creatorRepository = creatorRepository;
        _userRepository = userRepository;
        _voteRepository = voteRepository;
    }

    public async Task<CreatorMatchupDto?> GetRandomMatchupAsync(
        CancellationToken cancellationToken = default)
    {
        var first = await _creatorRepository.GetRandomActiveCreatorAsync(
            cancellationToken: cancellationToken);

        if (first is null)
            return null;

        var second = await _creatorRepository.GetRandomActiveCreatorAsync(
            excludeCreatorId: first.Id,
            cancellationToken: cancellationToken);

        if (second is null)
            return null;

        return new CreatorMatchupDto
        {
            FirstCreator = MapCreator(first),
            SecondCreator = MapCreator(second)
        };
    }

    public async Task<(bool Success, string Message)> CastVoteAsync(
        Guid userId,
        Guid firstCreatorId,
        Guid secondCreatorId,
        Guid selectedCreatorId,
        CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty ||
            firstCreatorId == Guid.Empty ||
            secondCreatorId == Guid.Empty ||
            firstCreatorId == secondCreatorId)
        {
            return (false, "This matchup is invalid. Please try another.");
        }

        if (selectedCreatorId != firstCreatorId &&
            selectedCreatorId != secondCreatorId)
        {
            return (false, "Please select one of the two creators shown.");
        }

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null || !user.IsActive)
        {
            return (
                false,
                "Your account could not be verified. Please sign in again.");
        }

        var creatorIds = new[]
        {
            firstCreatorId,
            secondCreatorId
        };

        var activeCreators =
            await _creatorRepository.GetActiveByIdsAsync(
                creatorIds,
                cancellationToken);

        if (activeCreators.Count != 2)
        {
            return (
                false,
                "One of these creators is no longer available.");
        }

        // Always store the matchup in a consistent order.
        // This prevents voting twice by reversing the creator order.
        var orderedIds = creatorIds
            .OrderBy(id => id)
            .ToArray();

        var alreadyVoted = await _voteRepository.HasUserVotedAsync(
            userId,
            orderedIds[0],
            orderedIds[1],
            cancellationToken);

        if (alreadyVoted)
        {
            return (
                false,
                "You've already voted in this matchup.");
        }

        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FirstCreatorId = orderedIds[0],
            SecondCreatorId = orderedIds[1],
            SelectedCreatorId = selectedCreatorId,
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(vote);

        var saved = await _voteRepository.SaveChangesAsync(
            cancellationToken);

        if (!saved)
        {
            return (
                false,
                "Your vote could not be saved. You may have already voted. Please try again.");
        }

        return (
            true,
            "Your vote has been recorded. Thank you!");
    }

    public Task<IReadOnlyList<LeaderboardEntryDto>> GetLeaderboardAsync(
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        take = Math.Clamp(take, 1, 100);

        return _voteRepository.GetLeaderboardAsync(
            take,
            cancellationToken);
    }

    private static CreatorVoteCardDto MapCreator(Creator creator)
    {
        return new CreatorVoteCardDto
        {
            Id = creator.Id,
            DisplayName = creator.DisplayName,
            Slug = creator.Slug,
            ProfileImageUrl = creator.ProfileImageUrl,
            Tagline = creator.Tagline
        };
    }
}
