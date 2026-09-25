using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class Creator
    {
        public Guid Id { get; set; }

        public string DisplayName { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PlatformProfile> PlatformProfiles { get; set; } = [];
        public ICollection<CreatorCategory> Categories { get; set; } = [];
        public ICollection<CreatorTag> Tags { get; set; } = [];
        public ICollection<CreatorAttribute> CreatorAttributeDefinition { get; set; } = [];
    }
}
