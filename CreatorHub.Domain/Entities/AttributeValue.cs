using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class AttributeValue
    {
        public Guid Id { get; set; }

        public Guid AttributeId { get; set; }

        public string Value { get; set; } = null!;
        public string Slug { get; set; } = null!;

        public CreatorAttributeDefinition CreatorAttributeDefinition { get; set; } = null!;

        public ICollection<CreatorAttribute> Creators { get; set; } = [];
    }
}
