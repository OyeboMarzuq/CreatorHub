
namespace CreatorHub.Domain.Entities
{
    public class PlatformProfile
    {
        public Guid Id { get; set; }

        public Guid CreatorId { get; set; }
        public Guid PlatformId { get; set; }

        public string Username { get; set; } = null!;
        public string ProfileUrl { get; set; } = null!;

        public string? ExternalId { get; set; }

        public string? ProfileImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastImportedAt { get; set; }

        public Creator Creator { get; set; } = null!;
        public Platform Platform { get; set; } = null!;
    }
}
