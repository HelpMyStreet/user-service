using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddDateLastLogin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateLastLogin",
                schema: "User",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateLastLoginChecked",
                schema: "User",
                table: "User",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateLastLogin",
                schema: "User",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DateLastLoginChecked",
                schema: "User",
                table: "User");
        }
    }
}
