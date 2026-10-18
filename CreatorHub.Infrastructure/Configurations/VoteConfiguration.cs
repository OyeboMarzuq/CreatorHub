
using CreatorHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CreatorHub.Infrastructure.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CreatedAt);

        builder.HasIndex(x => x.SelectedCreatorId);

        // A user can vote only once for a canonically ordered matchup.
        builder.HasIndex(x => new
        {
            x.UserId,
            x.FirstCreatorId,
            x.SecondCreatorId
        })
        .IsUnique();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Creator>()
            .WithMany()
            .HasForeignKey(x => x.FirstCreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Creator>()
            .WithMany()
            .HasForeignKey(x => x.SecondCreatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Creator>()
            .WithMany()
            .HasForeignKey(x => x.SelectedCreatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
