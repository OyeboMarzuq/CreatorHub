using CreatorHub.Application.DTOs.Platforms;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Services;

public class PlatformService : IPlatformService
{
    private readonly IPlatformRepository _platformRepository;

    public PlatformService(IPlatformRepository platformRepository)
    {
        _platformRepository = platformRepository;
    }

    public async Task<PlatformDto?> GetByIdAsync(Guid id)
    {
        var platform = await _platformRepository.GetByIdAsync(id);

        return platform == null ? null : MapToDto(platform);
    }

    public async Task<PlatformDto?> GetBySlugAsync(string slug)
    {
        var platform = await _platformRepository.GetBySlugAsync(slug);

        return platform == null ? null : MapToDto(platform);
    }

    public async Task<List<PlatformDto>> GetAllAsync()
    {
        var platforms = await _platformRepository.GetAllAsync();

        return platforms
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PlatformDto> CreateAsync(CreatePlatformDto request)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _platformRepository.ExistsBySlugAsync(slug))
        {
            throw new InvalidOperationException(
                $"A platform with the slug '{slug}' already exists.");
        }

        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            BaseUrl = request.BaseUrl,
            IsActive = true
        };

        await _platformRepository.AddAsync(platform);
        await _platformRepository.SaveChangesAsync();

        return MapToDto(platform);
    }

    private static PlatformDto MapToDto(Platform platform)
    {
        return new PlatformDto
        {
            Id = platform.Id,
            Name = platform.Name,
            Slug = platform.Slug,
            BaseUrl = platform.BaseUrl,
            IsActive = platform.IsActive
        };
    }
}