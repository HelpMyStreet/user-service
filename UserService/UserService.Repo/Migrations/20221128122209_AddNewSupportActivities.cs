using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddNewSupportActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 34, "Accommodation" },
                    { 35, "AdvertisingRoles" },
                    { 36, "NHSSteward" },
                    { 37, "NHSTransport" },
                    { 38, "NHSCheckInAndChat" },
                    { 39, "NHSCheckInAndChatPlus" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 35);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 36);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 37);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 38);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 39);
        }
    }
}
