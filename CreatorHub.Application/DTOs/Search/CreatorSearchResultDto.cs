using CreatorHub.Application.DTOs.Creators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.DTOs.Search
{
    public class CreatorSearchResultDto
    {
        public List<CreatorDto> Creators { get; set; } = [];

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
