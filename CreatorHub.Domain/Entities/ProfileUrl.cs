using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class ProfileUrl
    {
        public Guid Id { get; set; }

        public string Username { get; set; } = null!;

        public string Url { get; set; } = null!;

        public string Source { get; set; } = null!;

        public int SourcePage { get; set; }

        public bool IsProcessed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }
    }
}
