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
    public class PlatformProfileConfiguration
    : IEntityTypeConfiguration<PlatformProfile>
    {
        public void Configure(EntityTypeBuilder<PlatformProfile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Username)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ProfileUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.ExternalId)
                .HasMaxLength(200);

            builder.Property(x => x.ProfileImageUrl)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Creator)
                .WithMany(x => x.PlatformProfiles)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Platform)
                .WithMany(x => x.Profiles)
                .HasForeignKey(x => x.PlatformId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new
            {
                x.PlatformId,
                x.Username
            }).IsUnique();

            builder.HasIndex(x => new
            {
                x.PlatformId,
                x.ExternalId
            });
        }
    }
}