using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreatorHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileUrlImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ProfileUrls",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ProfileUrls");
        }
    }
}
