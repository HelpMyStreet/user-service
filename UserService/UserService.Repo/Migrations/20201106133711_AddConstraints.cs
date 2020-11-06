using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_FirebaseUID",
                schema: "User",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "UC_EmailAddress",
                schema: "UserPersonal",
                table: "PersonalDetails",
                column: "EmailAddress",
                unique: true,
                filter: "[EmailAddress] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UC_FirebaseUID",
                schema: "User",
                table: "User",
                column: "FirebaseUID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UC_EmailAddress",
                schema: "UserPersonal",
                table: "PersonalDetails");

            migrationBuilder.DropIndex(
                name: "UC_FirebaseUID",
                schema: "User",
                table: "User");

            migrationBuilder.CreateIndex(
                name: "ix_FirebaseUID",
                schema: "User",
                table: "User",
                column: "FirebaseUID");
        }
    }
}
