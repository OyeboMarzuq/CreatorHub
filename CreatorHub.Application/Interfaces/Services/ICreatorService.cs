using CreatorHub.Application.DTOs.Creators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Services
{
    public interface ICreatorService
    {
        Task<CreatorDto?> GetByIdAsync(Guid id);

        Task<CreatorDto?> GetBySlugAsync(string slug);

        Task<List<CreatorDto>> GetAllAsync();

        Task<CreatorDto> CreateAsync(CreateCreatorDto request);
    }
}
