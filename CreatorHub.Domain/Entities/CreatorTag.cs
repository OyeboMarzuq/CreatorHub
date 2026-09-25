using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class CreatorTag
    {
        public Guid CreatorId { get; set; }
        public Guid TagId { get; set; }

        public Creator Creator { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
