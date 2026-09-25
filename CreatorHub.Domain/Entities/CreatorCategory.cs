using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class CreatorCategory
    {
        public Guid CreatorId { get; set; }
        public Guid CategoryId { get; set; }

        public Creator Creator { get; set; } = null!;
        public Category Category { get; set; } = null!;
    }
}
