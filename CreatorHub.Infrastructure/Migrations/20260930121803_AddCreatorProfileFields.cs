using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreatorHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatorProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgeRating",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Characteristics",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HeightCm",
                table: "Creators",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Languages",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sexuality",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tagline",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThreeWords",
                table: "Creators",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WeightKg",
                table: "Creators",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgeRating",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Characteristics",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "HeightCm",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Languages",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Sexuality",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "Tagline",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "ThreeWords",
                table: "Creators");

            migrationBuilder.DropColumn(
                name: "WeightKg",
                table: "Creators");
        }
    }
}
