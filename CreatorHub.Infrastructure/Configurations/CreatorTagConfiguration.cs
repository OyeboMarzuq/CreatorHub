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
    public class CreatorTagConfiguration
    : IEntityTypeConfiguration<CreatorTag>
    {
        public void Configure(EntityTypeBuilder<CreatorTag> builder)
        {
            builder.HasKey(x => new
            {
                x.CreatorId,
                x.TagId
            });

            builder.HasOne(x => x.Creator)
                .WithMany(x => x.Tags)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tag)
                .WithMany(x => x.Creators)
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
