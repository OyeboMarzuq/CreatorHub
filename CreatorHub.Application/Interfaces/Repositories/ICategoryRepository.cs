using CreatorHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);

        Task<Category?> GetBySlugAsync(string slug);

        Task<List<Category>> GetAllAsync();

        Task AddAsync(Category category);

        Task<bool> ExistsBySlugAsync(string slug);

        Task SaveChangesAsync();
    }
}
