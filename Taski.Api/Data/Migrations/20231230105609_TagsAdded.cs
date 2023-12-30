using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taski.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class TagsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "Stories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ProjectTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoryTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTagAssociations",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTagAssociations", x => new { x.ProjectId, x.ProjectTagId });
                    table.ForeignKey(
                        name: "FK_ProjectTagAssociations_ProjectTags_ProjectTagId",
                        column: x => x.ProjectTagId,
                        principalTable: "ProjectTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTagAssociations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_TagId",
                table: "Stories",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTagAssociations_ProjectTagId",
                table: "ProjectTagAssociations",
                column: "ProjectTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stories_StoryTags_TagId",
                table: "Stories",
                column: "TagId",
                principalTable: "StoryTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stories_StoryTags_TagId",
                table: "Stories");

            migrationBuilder.DropTable(
                name: "ProjectTagAssociations");

            migrationBuilder.DropTable(
                name: "StoryTags");

            migrationBuilder.DropTable(
                name: "ProjectTags");

            migrationBuilder.DropIndex(
                name: "IX_Stories_TagId",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Stories");
        }
    }
}
