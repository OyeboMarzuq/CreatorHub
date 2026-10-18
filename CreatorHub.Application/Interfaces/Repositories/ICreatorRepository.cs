using CreatorHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Repositories
{
    public interface ICreatorRepository
    {
        Task<Creator?> GetByIdAsync(Guid id);

        Task<Creator?> GetBySlugAsync(string slug);

        Task<List<Creator>> GetAllAsync();

        Task<Creator?> GetByPlatformProfileAsync(
            Guid platformId,
            string externalId);

        Task<Creator?> GetRandomActiveCreatorAsync(
    Guid? excludeCreatorId = null,
    CancellationToken cancellationToken = default);

        Task<List<Creator>> GetActiveByIdsAsync(
            IReadOnlyCollection<Guid> ids,
            CancellationToken cancellationToken = default);

        Task<PlatformProfile?> GetPlatformProfileByUsernameAsync(
            Guid platformId,
            string username);

        Task AddPlatformProfileAsync(PlatformProfile profile);

        Task<Platform?> GetPlatformBySlugAsync(string slug);

        Task AddPlatformAsync(Platform platform);

        Task AddAsync(Creator creator);

        Task UpdateAsync(Creator creator);

        Task<bool> ExistsBySlugAsync(string slug);

        Task SaveChangesAsync();
    }
}