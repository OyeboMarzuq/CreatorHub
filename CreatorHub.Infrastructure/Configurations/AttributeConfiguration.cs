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
    public class AttributeConfiguration
    : IEntityTypeConfiguration<CreatorAttributeDefinition>
    {
        public void Configure(EntityTypeBuilder<CreatorAttributeDefinition> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Slug)
                .IsUnique();
        }
    }
}
