
namespace CreatorHub.Web.Models;

public class DashboardViewModel
{
    public int TotalCreators { get; set; }
    public int ActiveCreators { get; set; }
    public int TotalPlatforms { get; set; }
    public int TotalProfileUrls { get; set; }
    public int ProcessedProfileUrls { get; set; }
    public int TotalMembers { get; set; }

    // Voting statistics
    public int TotalVotes { get; set; }
    public int TotalMatchups { get; set; }

    // Dashboard leaderboard
    public List<LeaderboardRow> TopCreators { get; set; } = [];

    public List<SourceProgress> Sources { get; set; } = [];
    public List<MemberRow> RecentMembers { get; set; } = [];

    public class LeaderboardRow
    {
        public int Rank { get; set; }
        public Guid CreatorId { get; set; }
        public string DisplayName { get; set; } = "";
        public string Slug { get; set; } = "";
        public string? ProfileImageUrl { get; set; }
        public int VoteCount { get; set; }
    }

    public class SourceProgress
    {
        public string Source { get; set; } = "";
        public int Total { get; set; }
        public int Processed { get; set; }

        public int Percent => Total > 0
            ? Math.Clamp((int)Math.Round(Processed * 100.0 / Total), 0, 100)
            : 0;
    }

    public class MemberRow
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Region { get; set; } = "";
        public string Gender { get; set; } = "";
        public DateTime JoinedAt { get; set; }
    }
}
