
using CreatorHub.Application.DTOs.Creators;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Services;

public class CreatorService : ICreatorService
{
    private readonly ICreatorRepository _creatorRepository;

    public CreatorService(ICreatorRepository creatorRepository)
    {
        _creatorRepository = creatorRepository;
    }

    public async Task<CreatorDto?> GetByIdAsync(Guid id)
    {
        var creator = await _creatorRepository.GetByIdAsync(id);

        return creator == null ? null : MapToDto(creator);
    }

    public async Task<CreatorDto?> GetBySlugAsync(string slug)
    {
        var creator = await _creatorRepository.GetBySlugAsync(slug);

        return creator == null ? null : MapToDto(creator);
    }

    public async Task<List<CreatorDto>> GetAllAsync()
    {
        var creators = await _creatorRepository.GetAllAsync();

        return creators.Select(MapToDto).ToList();
    }

    public async Task<CreatorDto> CreateAsync(CreateCreatorDto request)
    {
        var slug = string.IsNullOrWhiteSpace(request.Slug)
            ? GenerateSlug(request.DisplayName)
            : request.Slug.Trim().ToLowerInvariant();

        if (await _creatorRepository.ExistsBySlugAsync(slug))
        {
            throw new InvalidOperationException(
                $"A creator with the slug '{slug}' already exists.");
        }

        var creator = new Creator
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            Slug = slug,
            Age = request.Age,
            Gender = request.Gender,
            Country = request.Country,
            Region = request.Region,
            ProfileImageUrl = request.ProfileImageUrl,
            Bio = request.Bio,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _creatorRepository.AddAsync(creator);
        await _creatorRepository.SaveChangesAsync();

        return MapToDto(creator);
    }

    private static CreatorDto MapToDto(Creator creator)
    {
        // Choose a platform profile that has a usable original URL.
        var platformProfile = creator.PlatformProfiles?
            .Where(p => p.IsActive)
            .Where(p => !string.IsNullOrWhiteSpace(p.ProfileUrl))
            .OrderBy(p => p.Platform == null ? 1 : 0)
            .FirstOrDefault();

        // Fall back to any profile with a URL if none is active.
        platformProfile ??= creator.PlatformProfiles?
            .FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.ProfileUrl));

        return new CreatorDto
        {
            Id = creator.Id,
            DisplayName = creator.DisplayName,
            Slug = creator.Slug,
            Age = creator.Age,
            Gender = creator.Gender,
            Country = creator.Country,
            Region = creator.Region,

            ProfileImageUrl = FirstNonEmpty(
                creator.ProfileImageUrl,
                platformProfile?.ProfileImageUrl),

            // This is the original profile page, not the image CDN.
            ProfileUrl = GetSafeHttpUrl(platformProfile?.ProfileUrl),

            PlatformName = platformProfile?.Platform?.Name,

            Bio = creator.Bio,
            IsActive = creator.IsActive,

            Categories = creator.Categories?
                .Where(x => x.Category != null)
                .Select(x => x.Category.Name)
                .ToList() ?? [],

            Tags = creator.Tags?
                .Where(x => x.Tag != null)
                .Select(x => x.Tag.Name)
                .ToList() ?? []
        };
    }

    private static string? FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value =>
            !string.IsNullOrWhiteSpace(value));
    }

    private static string? GetSafeHttpUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttps ||
             uri.Scheme == Uri.UriSchemeHttp))
        {
            return uri.AbsoluteUri;
        }

        return null;
    }

    private static string GenerateSlug(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-");
    }
}
