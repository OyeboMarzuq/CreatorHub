using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Application.DTOs.Categories
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        public bool IsActive { get; set; }
        public bool IsIndexable { get; set; }
    }
}
