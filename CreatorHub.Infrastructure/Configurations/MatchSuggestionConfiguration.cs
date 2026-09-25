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
    public class MatchSuggestionConfiguration
    : IEntityTypeConfiguration<MatchSuggestion>
    {
        public void Configure(EntityTypeBuilder<MatchSuggestion> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ConfidenceScore)
                .HasPrecision(5, 2);

            builder.Property(x => x.MatchReasons)
                .HasMaxLength(4000);

            builder.HasOne(x => x.SourceProfile)
                .WithMany()
                .HasForeignKey(x => x.SourceProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.TargetProfile)
                .WithMany()
                .HasForeignKey(x => x.TargetProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => new
            {
                x.SourceProfileId,
                x.TargetProfileId
            }).IsUnique();
        }
    }
}