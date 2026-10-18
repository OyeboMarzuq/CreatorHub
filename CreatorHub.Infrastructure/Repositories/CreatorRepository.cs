using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Domain.Entities;
using CreatorHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Infrastructure.Repositories
{
    public class CreatorRepository : ICreatorRepository
    {
        private readonly CreatorHubDbContext _context;

        public CreatorRepository(CreatorHubDbContext context)
        {
            _context = context;
        }

        public async Task<Creator?> GetByIdAsync(Guid id)
        {
            return await _context.Creators
                .Include(x => x.Categories)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Tags)
                    .ThenInclude(x => x.Tag)
                .Include(x => x.PlatformProfiles)
                    .ThenInclude(x => x.Platform)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Creator?> GetBySlugAsync(string slug)
        {
            return await _context.Creators
                .Include(x => x.Categories)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Tags)
                    .ThenInclude(x => x.Tag)
                .Include(x => x.PlatformProfiles)
                    .ThenInclude(x => x.Platform)
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<List<Creator>> GetAllAsync()
        {
            return await _context.Creators
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Creator?> GetByPlatformProfileAsync(
            Guid platformId,
            string externalId)
        {
            return await _context.Creators
                .Include(x => x.PlatformProfiles)
                .FirstOrDefaultAsync(x =>
                    x.PlatformProfiles.Any(p =>
                        p.PlatformId == platformId &&
                        p.ExternalId == externalId));
        }

        public async Task<PlatformProfile?> GetPlatformProfileByUsernameAsync(
            Guid platformId,
            string username)
        {
            return await _context.PlatformProfiles
                .FirstOrDefaultAsync(x =>
                    x.PlatformId == platformId &&
                    x.Username == username);
        }

        public async Task AddPlatformProfileAsync(PlatformProfile profile)
        {
            await _context.PlatformProfiles.AddAsync(profile);
        }

        public async Task<Platform?> GetPlatformBySlugAsync(string slug)
        {
            return await _context.Platforms
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task AddPlatformAsync(Platform platform)
        {
            await _context.Platforms.AddAsync(platform);
        }

        public async Task AddAsync(Creator creator)
        {
            await _context.Creators.AddAsync(creator);
        }

        public Task UpdateAsync(Creator creator)
        {
            _context.Creators.Update(creator);
            return Task.CompletedTask;
        }
        public async Task<Creator?> GetRandomActiveCreatorAsync(
    Guid? excludeCreatorId = null,
    CancellationToken cancellationToken = default)
        {
            var query = _context.Creators
                .AsNoTracking()
                .Where(c => c.IsActive);

            if (excludeCreatorId.HasValue)
            {
                query = query.Where(c => c.Id != excludeCreatorId.Value);
            }

            var count = await query.CountAsync(cancellationToken);

            if (count == 0)
                return null;

            var randomIndex = Random.Shared.Next(count);

            return await query
                .OrderBy(c => c.Id)
                .Skip(randomIndex)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<Creator>> GetActiveByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken = default)
        {
            return await _context.Creators
                .AsNoTracking()
                .Where(c => ids.Contains(c.Id) && c.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsBySlugAsync(string slug)
        {
            return await _context.Creators
                .AnyAsync(x => x.Slug == slug);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
