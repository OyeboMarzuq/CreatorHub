using CreatorHub.Application.DTOs.Search;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreatorHub.Infrastructure.Repositories;

public class CreatorSearchRepository : ICreatorSearchRepository
{
    private readonly CreatorHubDbContext _context;

    public CreatorSearchRepository(CreatorHubDbContext context)
    {
        _context = context;
    }

    public async Task<(List<Creator> Creators, int TotalCount)> SearchAsync(
        CreatorSearchRequest request)
    {
        IQueryable<Creator> query = _context.Creators
            .AsNoTracking()
            .Where(x => x.IsActive);

        // Text search
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var search = request.Query.Trim();

            query = query.Where(x =>
                x.DisplayName.Contains(search) ||
                (x.Bio != null && x.Bio.Contains(search)));
        }

        // Age
        if (request.MinAge.HasValue)
        {
            query = query.Where(x =>
                x.Age >= request.MinAge.Value);
        }

        if (request.MaxAge.HasValue)
        {
            query = query.Where(x =>
                x.Age <= request.MaxAge.Value);
        }

        // Gender
        if (!string.IsNullOrWhiteSpace(request.Gender))
        {
            query = query.Where(x =>
                x.Gender == request.Gender);
        }

        // Country
        if (!string.IsNullOrWhiteSpace(request.Country))
        {
            query = query.Where(x =>
                x.Country == request.Country);
        }

        // Region
        if (!string.IsNullOrWhiteSpace(request.Region))
        {
            query = query.Where(x =>
                x.Region == request.Region);
        }

        // Category
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var category = request.Category.Trim();

            query = query.Where(x =>
                x.Categories.Any(c =>
                    c.Category.Slug == category));
        }

        // Tags / hashtags
        if (request.Tags.Count > 0)
        {
            foreach (var tag in request.Tags)
            {
                var normalizedTag = tag.Trim();

                query = query.Where(x =>
                    x.Tags.Any(t =>
                        t.Tag.Slug == normalizedTag));
            }
        }

        // Platforms
        if (request.Platforms.Count > 0)
        {
            foreach (var platform in request.Platforms)
            {
                var normalizedPlatform = platform.Trim();

                query = query.Where(x =>
                    x.PlatformProfiles.Any(p =>
                        p.Platform.Slug == normalizedPlatform));
            }
        }

        var totalCount = await query.CountAsync();

        var creators = await query
            .OrderBy(x => x.DisplayName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(x => x.Categories)
                .ThenInclude(x => x.Category)
            .Include(x => x.Tags)
                .ThenInclude(x => x.Tag)
            .Include(x => x.PlatformProfiles)
                .ThenInclude(x => x.Platform)
            .ToListAsync();

        return (creators, totalCount);
    }
}