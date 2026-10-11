using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CreatorHubDbContext _context;

    public CategoryRepository(CreatorHubDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Categories
            .AnyAsync(x => x.Slug == slug);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}