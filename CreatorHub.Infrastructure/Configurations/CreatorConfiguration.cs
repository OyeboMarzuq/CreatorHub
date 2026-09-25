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
    public class CreatorConfiguration : IEntityTypeConfiguration<Creator>
    {
        public void Configure(EntityTypeBuilder<Creator> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Gender)
                .HasMaxLength(50);

            builder.Property(x => x.Country)
                .HasMaxLength(100);

            builder.Property(x => x.Region)
                .HasMaxLength(150);

            builder.Property(x => x.ProfileImageUrl)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.Slug)
                .IsUnique();

            builder.HasIndex(x => x.Country);

            builder.HasIndex(x => x.Age);

            builder.HasIndex(x => x.Gender);
        }
    }
}
