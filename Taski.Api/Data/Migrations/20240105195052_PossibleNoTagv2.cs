using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taski.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class PossibleNoTagv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_StoryTags_TagId",
                table: "Stories");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagId",
                table: "Stories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_StoryTags_TagId",
                table: "Stories",
                column: "TagId",
                principalTable: "StoryTags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_StoryTags_TagId",
                table: "Stories");

            migrationBuilder.AlterColumn<Guid>(
                name: "TagId",
                table: "Stories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_StoryTags_TagId",
                table: "Stories",
                column: "TagId",
                principalTable: "StoryTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
