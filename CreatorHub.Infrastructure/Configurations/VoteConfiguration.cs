using CreatorHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Infrastructure.Configurations
{
    public class VoteConfiguration : IEntityTypeConfiguration<Vote>
    {
        public void Configure(EntityTypeBuilder<Vote> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.CreatedAt);

            builder.HasIndex(x => x.SelectedCreatorId);
        }
    }
}
