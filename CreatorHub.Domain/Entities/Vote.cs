using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Domain.Entities
{
    public class Vote
    {
        public Guid Id { get; set; }

        public Guid FirstCreatorId { get; set; }
        public Guid SecondCreatorId { get; set; }

        public Guid SelectedCreatorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
