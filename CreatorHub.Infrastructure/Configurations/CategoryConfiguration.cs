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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.MetaTitle)
                .HasMaxLength(200);

            builder.Property(x => x.MetaDescription)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Slug)
                .IsUnique();
        }
    }
}
