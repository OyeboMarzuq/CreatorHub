using CreatorHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatorHub.Infrastructure.Data
{
    public class CreatorHubDbContext : DbContext
    {
        public CreatorHubDbContext(DbContextOptions<CreatorHubDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Creator> Creators => Set<Creator>();
        public DbSet<Platform> Platforms => Set<Platform>();
        public DbSet<PlatformProfile> PlatformProfiles => Set<PlatformProfile>();

        public DbSet<ProfileUrl> ProfileUrls { get; set; }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<CreatorCategory> CreatorCategories => Set<CreatorCategory>();

        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<CreatorTag> CreatorTags => Set<CreatorTag>();

        public DbSet<CreatorAttributeDefinition> CreatorAttributeDefinitions => Set<CreatorAttributeDefinition>();
        public DbSet<AttributeValue> AttributeValues => Set<AttributeValue>();
        public DbSet<CreatorAttribute> CreatorAttributes => Set<CreatorAttribute>();

        public DbSet<MatchSuggestion> MatchSuggestions => Set<MatchSuggestion>();

        public DbSet<Vote> Votes => Set<Vote>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(CreatorHubDbContext).Assembly);
        }
    }
}