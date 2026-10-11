using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Infrastructure.Repositories;

public class ProfileUrlRepository : IProfileUrlRepository
{
    private readonly CreatorHubDbContext _context;

    public ProfileUrlRepository(CreatorHubDbContext context)
    {
        _context = context;
    }

    public async Task AddRangeAsync(
        IEnumerable<ProfileUrl> profileUrls)
    {
        await _context.ProfileUrls.AddRangeAsync(profileUrls);
    }

    public async Task<List<ProfileUrl>> GetUnprocessedAsync(
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        return await _context.ProfileUrls
            .Where(x => !x.IsProcessed)
            .OrderBy(x => x.CreatedAt)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}