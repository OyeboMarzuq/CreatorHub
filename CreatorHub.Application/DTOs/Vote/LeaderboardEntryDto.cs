
namespace CreatorHub.Application.DTOs.Voting;

public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public Guid CreatorId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public int VoteCount { get; set; }
}
