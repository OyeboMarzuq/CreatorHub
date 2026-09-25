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
    public class CreatorCategoryConfiguration
    : IEntityTypeConfiguration<CreatorCategory>
    {
        public void Configure(EntityTypeBuilder<CreatorCategory> builder)
        {
            builder.HasKey(x => new
            {
                x.CreatorId,
                x.CategoryId
            });

            builder.HasOne(x => x.Creator)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Category)
                .WithMany(x => x.Creators)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
