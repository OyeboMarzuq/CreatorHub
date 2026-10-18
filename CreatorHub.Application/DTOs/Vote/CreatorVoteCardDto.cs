
namespace CreatorHub.Application.DTOs.Voting;

public class CreatorVoteCardDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ProfileImageUrl { get; set; }
    public string? Tagline { get; set; }
}
