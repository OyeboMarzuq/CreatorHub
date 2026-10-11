using CreatorHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Repositories
{
    public interface IPlatformRepository
    {
        Task<Platform?> GetByIdAsync(Guid id);

        Task<Platform?> GetBySlugAsync(string slug);

        Task<List<Platform>> GetAllAsync();

        Task AddAsync(Platform platform);

        Task<bool> ExistsBySlugAsync(string slug);

        Task SaveChangesAsync();
    }
}
