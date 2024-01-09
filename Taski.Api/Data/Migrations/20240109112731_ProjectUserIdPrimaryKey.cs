using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taski.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProjectUserIdPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProjectAssociations",
                table: "UserProjectAssociations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProjectAssociations",
                table: "UserProjectAssociations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectAssociations_UserId",
                table: "UserProjectAssociations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProjectAssociations",
                table: "UserProjectAssociations");

            migrationBuilder.DropIndex(
                name: "IX_UserProjectAssociations_UserId",
                table: "UserProjectAssociations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProjectAssociations",
                table: "UserProjectAssociations",
                columns: new[] { "UserId", "ProjectId" });
        }
    }
}
