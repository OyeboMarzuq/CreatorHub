using CreatorHub.Application.DTOs.Categories;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryDto?> GetBySlugAsync(string slug)
    {
        var category = await _categoryRepository.GetBySlugAsync(slug);

        return category == null ? null : MapToDto(category);
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();

        return categories
            .Select(MapToDto)
            .ToList();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto request)
    {
        var slug = request.Slug.Trim().ToLowerInvariant();

        if (await _categoryRepository.ExistsBySlugAsync(slug))
        {
            throw new InvalidOperationException(
                $"A category with the slug '{slug}' already exists.");
        }

        var category = new Category
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

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return MapToDto(category);
    }

    private static CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            MetaTitle = category.MetaTitle,
            MetaDescription = category.MetaDescription,
            IsActive = category.IsActive,
            IsIndexable = category.IsIndexable
        };
    }
}