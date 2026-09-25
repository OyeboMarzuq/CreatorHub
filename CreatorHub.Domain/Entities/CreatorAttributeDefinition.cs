using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class CreatorAttributeDefinition
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<AttributeValue> Values { get; set; } = [];
    }
}
