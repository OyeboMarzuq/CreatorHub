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
    public class AttributeValueConfiguration
    : IEntityTypeConfiguration<AttributeValue>
    {
        public void Configure(EntityTypeBuilder<AttributeValue> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasOne(x => x.CreatorAttributeDefinition)
                .WithMany(x => x.Values)
                .HasForeignKey(x => x.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new
            {
                x.AttributeId,
                x.Slug
            }).IsUnique();
        }
    }
}
