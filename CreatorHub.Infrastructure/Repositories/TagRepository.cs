using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Infrastructure.Repositories;

public class TagRepository : ITagRepository
{
    private readonly CreatorHubDbContext _context;

    public TagRepository(CreatorHubDbContext context)
    {
        _context = context;
    }

    public async Task<Tag?> GetByIdAsync(Guid id)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Tag?> GetBySlugAsync(string slug)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<List<Tag>> GetAllAsync()
    {
        return await _context.Tags
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
    }

    public async Task<bool> ExistsBySlugAsync(string slug)
    {
        return await _context.Tags
            .AnyAsync(x => x.Slug == slug);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}