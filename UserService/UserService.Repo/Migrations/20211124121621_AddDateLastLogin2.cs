using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddDateLastLogin2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLastLoginChecked",
                schema: "User",
                table: "User",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLastLoginChecked",
                schema: "User",
                table: "User",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
