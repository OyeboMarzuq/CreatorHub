using CreatorHub.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class MatchSuggestion
    {
        public Guid Id { get; set; }

        public Guid SourceProfileId { get; set; }
        public Guid TargetProfileId { get; set; }

        public decimal ConfidenceScore { get; set; }

        public string? MatchReasons { get; set; }

        public MatchStatus Status { get; set; } = MatchStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }

        public PlatformProfile SourceProfile { get; set; } = null!;
        public PlatformProfile TargetProfile { get; set; } = null!;
    }
}
