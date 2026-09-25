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
    public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.BaseUrl)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
