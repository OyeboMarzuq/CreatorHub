using CreatorHub.Application.DTOs.Creators;
using CreatorHub.Application.DTOs.Search;
using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Domain.Entities;

namespace CreatorHub.Application.Services;

public class CreatorSearchService : ISearchService
{
    private readonly ICreatorSearchRepository _searchRepository;

    public CreatorSearchService(
        ICreatorSearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<CreatorSearchResultDto> SearchAsync(
        CreatorSearchRequest request)
    {
        request.Page = request.Page < 1
            ? 1
            : request.Page;

        request.PageSize = request.PageSize switch
        {
            <= 0 => 24,
            > 100 => 100,
            _ => request.PageSize
        };

        var result = await _searchRepository.SearchAsync(request);

        var creators = result.Creators
            .Select(x => new CreatorDto
            {
                Id = x.Id,
                DisplayName = x.DisplayName,
                Slug = x.Slug,
                Age = x.Age,
                Gender = x.Gender,
                Country = x.Country,
                Region = x.Region,
                ProfileImageUrl = x.ProfileImageUrl,
                Bio = x.Bio,
                IsActive = x.IsActive,

                Categories = x.Categories
                    .Select(c => c.Category.Name)
                    .ToList(),

                Tags = x.Tags
                    .Select(t => t.Tag.Name)
                    .ToList()
            })
            .ToList();

        return new CreatorSearchResultDto
        {
            Creators = creators,
            TotalCount = result.TotalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}