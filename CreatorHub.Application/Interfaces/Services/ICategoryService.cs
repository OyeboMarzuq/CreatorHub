using CreatorHub.Application.DTOs.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto?> GetByIdAsync(Guid id);

        Task<CategoryDto?> GetBySlugAsync(string slug);

        Task<List<CategoryDto>> GetAllAsync();

        Task<CategoryDto> CreateAsync(CreateCategoryDto request);
    }
}
