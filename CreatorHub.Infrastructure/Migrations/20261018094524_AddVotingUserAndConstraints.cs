using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreatorHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVotingUserAndConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Votes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Votes_FirstCreatorId",
                table: "Votes",
                column: "FirstCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_SecondCreatorId",
                table: "Votes",
                column: "SecondCreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId_FirstCreatorId_SecondCreatorId",
                table: "Votes",
                columns: new[] { "UserId", "FirstCreatorId", "SecondCreatorId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Creators_FirstCreatorId",
                table: "Votes",
                column: "FirstCreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Creators_SecondCreatorId",
                table: "Votes",
                column: "SecondCreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Creators_SelectedCreatorId",
                table: "Votes",
                column: "SelectedCreatorId",
                principalTable: "Creators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Users_UserId",
                table: "Votes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Creators_FirstCreatorId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Creators_SecondCreatorId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Creators_SelectedCreatorId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Users_UserId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_FirstCreatorId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_SecondCreatorId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId_FirstCreatorId_SecondCreatorId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Votes");
        }
    }
}
