
namespace CreatorHub.Application.DTOs.Creators;

public class CreatorDto
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

    // Original profile page, not the CDN image URL.
    public string? ProfileUrl { get; set; }
    public string? PlatformName { get; set; }

    public bool IsActive { get; set; }

    public List<string> Categories { get; set; } = [];
    public List<string> Tags { get; set; } = [];
}
