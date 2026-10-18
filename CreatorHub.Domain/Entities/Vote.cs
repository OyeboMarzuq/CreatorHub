
namespace CreatorHub.Domain.Entities;

public class Vote
{
    public Guid Id { get; set; }

    // The registered user who cast this vote.
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // The two creators in this matchup.
    public Guid FirstCreatorId { get; set; }
    public Guid SecondCreatorId { get; set; }

    // Must be either FirstCreatorId or SecondCreatorId.
    public Guid SelectedCreatorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
