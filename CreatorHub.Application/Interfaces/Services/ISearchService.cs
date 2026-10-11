using CreatorHub.Application.DTOs.Creators;
using CreatorHub.Application.DTOs.Search;
using CreatorHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.Interfaces.Services
{
    public interface ISearchService
    {
        Task<CreatorSearchResultDto> SearchAsync(
            CreatorSearchRequest request);
    }
}
