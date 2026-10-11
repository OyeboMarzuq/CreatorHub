using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.DTOs.Creators
{
    public class CreateCreatorDto
    {
        public string DisplayName { get; set; } = null!;
        public string? Slug { get; set; }

        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }

        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
    }
}
