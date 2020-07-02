using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddReferringGroupID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReferringGroupId",
                schema: "User",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Source",
                schema: "User",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferringGroupId",
                schema: "User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Source",
                schema: "User",
                table: "User");
        }
    }
}
