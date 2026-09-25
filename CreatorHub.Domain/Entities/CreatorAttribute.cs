using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class CreatorAttribute
    {
        public Guid CreatorId { get; set; }
        public Guid AttributeValueId { get; set; }

        public Creator Creator { get; set; } = null!;
        public AttributeValue AttributeValue { get; set; } = null!;
    }
}
