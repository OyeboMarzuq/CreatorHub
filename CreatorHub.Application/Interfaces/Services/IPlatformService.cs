using CreatorHub.Application.DTOs.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Services
{
    public interface IPlatformService
    {
        Task<PlatformDto?> GetByIdAsync(Guid id);

        Task<PlatformDto?> GetBySlugAsync(string slug);

        Task<List<PlatformDto>> GetAllAsync();

        Task<PlatformDto> CreateAsync(CreatePlatformDto request);
    }
}
