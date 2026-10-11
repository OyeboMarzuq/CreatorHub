using CreatorHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Repositories
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(Guid id);

        Task<Tag?> GetBySlugAsync(string slug);

        Task<List<Tag>> GetAllAsync();

        Task AddAsync(Tag tag);

        Task<bool> ExistsBySlugAsync(string slug);

        Task SaveChangesAsync();
    }
}
