using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.DTOs.Platforms
{
    public class PlatformDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? BaseUrl { get; set; }

        public bool IsActive { get; set; }
    }
}
