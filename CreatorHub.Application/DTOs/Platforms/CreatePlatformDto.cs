using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.DTOs.Platforms
{
    public class CreatePlatformDto
    {
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? BaseUrl { get; set; }
    }
}
