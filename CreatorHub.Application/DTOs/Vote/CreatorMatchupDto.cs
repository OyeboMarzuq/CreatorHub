
namespace CreatorHub.Application.DTOs.Voting;

public class CreatorMatchupDto
{
    public CreatorVoteCardDto FirstCreator { get; set; } = new();
    public CreatorVoteCardDto SecondCreator { get; set; } = new();
}
