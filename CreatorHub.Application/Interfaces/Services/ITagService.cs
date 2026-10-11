using CreatorHub.Application.DTOs.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Services
{
    public interface ITagService
    {
        Task<TagDto?> GetByIdAsync(Guid id);

        Task<TagDto?> GetBySlugAsync(string slug);

        Task<List<TagDto>> GetAllAsync();

        Task<TagDto> CreateAsync(CreateTagDto request);
    }
}
