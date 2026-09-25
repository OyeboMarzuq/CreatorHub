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
    public class CreatorAttributeConfiguration
    : IEntityTypeConfiguration<CreatorAttribute>
    {
        public void Configure(EntityTypeBuilder<CreatorAttribute> builder)
        {
            builder.HasKey(x => new
            {
                x.CreatorId,
                x.AttributeValueId
            });

            builder.HasOne(x => x.Creator)
                .WithMany(x => x.CreatorAttributeDefinition)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AttributeValue)
                .WithMany(x => x.Creators)
                .HasForeignKey(x => x.AttributeValueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}