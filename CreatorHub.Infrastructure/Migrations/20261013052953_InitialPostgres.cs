using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreatorHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MetaTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MetaDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsIndexable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreatorAttributeDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorAttributeDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Creators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Region = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    HeightCm = table.Column<int>(type: "integer", nullable: true),
                    WeightKg = table.Column<decimal>(type: "numeric", nullable: true),
                    Tagline = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    Languages = table.Column<string>(type: "text", nullable: true),
                    Sexuality = table.Column<string>(type: "text", nullable: true),
                    ThreeWords = table.Column<string>(type: "text", nullable: true),
                    Characteristics = table.Column<string>(type: "text", nullable: true),
                    AgeRating = table.Column<string>(type: "text", nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileUrls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    SourcePage = table.Column<int>(type: "integer", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    MetaTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    MetaDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsIndexable = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Region = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "User"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    FailedLoginCount = table.Column<int>(type: "integer", nullable: false),
                    LockoutEndUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PasswordResetTokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PasswordResetExpiresUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstCreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SecondCreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    SelectedCreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttributeValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttributeValues_CreatorAttributeDefinitions_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "CreatorAttributeDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorCategories",
                columns: table => new
                {
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorCategories", x => new { x.CreatorId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CreatorCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorCategories_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlatformProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ProfileUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProfileImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    LastImportedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformProfiles_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformProfiles_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CreatorTags",
                columns: table => new
                {
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorTags", x => new { x.CreatorId, x.TagId });
                    table.ForeignKey(
                        name: "FK_CreatorTags_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CreatorAttributes",
                columns: table => new
                {
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttributeValueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreatorAttributes", x => new { x.CreatorId, x.AttributeValueId });
                    table.ForeignKey(
                        name: "FK_CreatorAttributes_AttributeValues_AttributeValueId",
                        column: x => x.AttributeValueId,
                        principalTable: "AttributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CreatorAttributes_Creators_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Creators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchSuggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfidenceScore = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    MatchReasons = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSuggestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchSuggestions_PlatformProfiles_SourceProfileId",
                        column: x => x.SourceProfileId,
                        principalTable: "PlatformProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSuggestions_PlatformProfiles_TargetProfileId",
                        column: x => x.TargetProfileId,
                        principalTable: "PlatformProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttributeValues_AttributeId_Slug",
                table: "AttributeValues",
                columns: new[] { "AttributeId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatorAttributeDefinitions_Slug",
                table: "CreatorAttributeDefinitions",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatorAttributes_AttributeValueId",
                table: "CreatorAttributes",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CreatorCategories_CategoryId",
                table: "CreatorCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Age",
                table: "Creators",
                column: "Age");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Country",
                table: "Creators",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Gender",
                table: "Creators",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Creators_Slug",
                table: "Creators",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreatorTags_TagId",
                table: "CreatorTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSuggestions_SourceProfileId_TargetProfileId",
                table: "MatchSuggestions",
                columns: new[] { "SourceProfileId", "TargetProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchSuggestions_Status",
                table: "MatchSuggestions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSuggestions_TargetProfileId",
                table: "MatchSuggestions",
                column: "TargetProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformProfiles_CreatorId",
                table: "PlatformProfiles",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformProfiles_PlatformId_ExternalId",
                table: "PlatformProfiles",
                columns: new[] { "PlatformId", "ExternalId" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformProfiles_PlatformId_Username",
                table: "PlatformProfiles",
                columns: new[] { "PlatformId", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Name",
                table: "Platforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Slug",
                table: "Platforms",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CreatedAt",
                table: "Votes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_SelectedCreatorId",
                table: "Votes",
                column: "SelectedCreatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreatorAttributes");

            migrationBuilder.DropTable(
                name: "CreatorCategories");

            migrationBuilder.DropTable(
                name: "CreatorTags");

            migrationBuilder.DropTable(
                name: "MatchSuggestions");

            migrationBuilder.DropTable(
                name: "ProfileUrls");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "AttributeValues");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "PlatformProfiles");

            migrationBuilder.DropTable(
                name: "CreatorAttributeDefinitions");

            migrationBuilder.DropTable(
                name: "Creators");

            migrationBuilder.DropTable(
                name: "Platforms");
        }
    }
}
