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

        if (creator == null)
            return null;

        return MapToDto(creator);
    }

    public async Task<CreatorDto?> GetBySlugAsync(string slug)
    {
        var creator = await _creatorRepository.GetBySlugAsync(slug);

        if (creator == null)
            return null;

        return MapToDto(creator);
    }

    public async Task<List<CreatorDto>> GetAllAsync()
    {
        var creators = await _creatorRepository.GetAllAsync();

        return creators
            .Select(MapToDto)
            .ToList();
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
        return new CreatorDto
        {
            Id = creator.Id,
            DisplayName = creator.DisplayName,
            Slug = creator.Slug,
            Age = creator.Age,
            Gender = creator.Gender,
            Country = creator.Country,
            Region = creator.Region,
            ProfileImageUrl = creator.ProfileImageUrl,
            Bio = creator.Bio,
            IsActive = creator.IsActive,

            Categories = creator.Categories
                .Select(x => x.Category.Name)
                .ToList(),

            Tags = creator.Tags
                .Select(x => x.Tag.Name)
                .ToList()
        };
    }

    private static string GenerateSlug(string value)
    {
        return value
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", "-");
    }
}