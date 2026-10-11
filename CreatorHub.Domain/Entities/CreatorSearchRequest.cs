using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class CreatorSearchRequest
    {
        public string? Query { get; set; }

        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }

        public string? Gender { get; set; }

        public string? Country { get; set; }

        public string? Region { get; set; }

        public string? Category { get; set; }

        public List<string> Tags { get; set; } = [];

        public List<string> Platforms { get; set; } = [];

        public Dictionary<string, List<string>> Attributes { get; set; } = [];

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 24;
    }
}
