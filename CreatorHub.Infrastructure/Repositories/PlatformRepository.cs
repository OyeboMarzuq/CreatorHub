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
    public class PlatformRepository : IPlatformRepository
    {
        private readonly CreatorHubDbContext _context;

        public PlatformRepository(CreatorHubDbContext context)
        {
            _context = context;
        }

        public async Task<Platform?> GetByIdAsync(Guid id)
        {
            return await _context.Platforms
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Platform?> GetBySlugAsync(string slug)
        {
            return await _context.Platforms
                .FirstOrDefaultAsync(x => x.Slug == slug);
        }

        public async Task<List<Platform>> GetAllAsync()
        {
            return await _context.Platforms
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Platform platform)
        {
            await _context.Platforms.AddAsync(platform);
        }

        public async Task<bool> ExistsBySlugAsync(string slug)
        {
            return await _context.Platforms
                .AnyAsync(x => x.Slug == slug);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}