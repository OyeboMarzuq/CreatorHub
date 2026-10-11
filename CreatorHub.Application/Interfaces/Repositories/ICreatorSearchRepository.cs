using CreatorHub.Application.DTOs.Search;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Interfaces.Repositories;

public interface ICreatorSearchRepository
{
    Task<(List<Creator> Creators, int TotalCount)> SearchAsync(
        CreatorSearchRequest request);
}