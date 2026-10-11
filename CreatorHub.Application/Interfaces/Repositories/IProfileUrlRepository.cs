using CreatorHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Repositories
{
    public interface IProfileUrlRepository
    {
        Task AddRangeAsync(IEnumerable<ProfileUrl> profileUrls);

        Task<List<ProfileUrl>> GetUnprocessedAsync(
            int batchSize,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync();
    }
}
