using CreatorHub.Application.DTOs.Tags;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<TagDto?> GetByIdAsync(Guid id)
    {
        var tag = await _tagRepository.GetByIdAsync(id);

        return tag == null ? null : MapToDto(tag);
    }

    public async Task<TagDto?> GetBySlugAsync(string slug)
    {
        var tag = await _tagRepository.GetBySlugAsync(slug);

        return tag == null ? null : MapToDto(tag);
    }

    public async Task<List<TagDto>> GetAllAsync()
    {
        var tags = await _tagRepository.GetAllAsync();

        return tags
            .Select(MapToDto)
            .ToList();
    }

    public async Task<TagDto> CreateAsync(CreateTagDto request)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _tagRepository.ExistsBySlugAsync(slug))
        {
            throw new InvalidOperationException(
                $"A tag with the slug '{slug}' already exists.");
        }

        var tag = new Tag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            MetaTitle = request.MetaTitle,
            MetaDescription = request.MetaDescription,
            IsActive = true,
            IsIndexable = request.IsIndexable
        };

        await _tagRepository.AddAsync(tag);
        await _tagRepository.SaveChangesAsync();

        return MapToDto(tag);
    }

    private static TagDto MapToDto(Tag tag)
    {
        return new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Slug = tag.Slug,
            Description = tag.Description,
            MetaTitle = tag.MetaTitle,
            MetaDescription = tag.MetaDescription,
            IsActive = tag.IsActive,
            IsIndexable = tag.IsIndexable
        };
    }
}