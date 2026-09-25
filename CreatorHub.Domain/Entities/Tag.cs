using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public string? Description { get; set; }

        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsIndexable { get; set; } = true;

        public ICollection<CreatorTag> Creators { get; set; } = [];
    }
}
