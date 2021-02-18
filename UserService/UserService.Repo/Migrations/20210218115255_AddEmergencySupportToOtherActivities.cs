using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddEmergencySupportToOtherActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 17);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 18);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 19);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 20);

            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[] { 25, "EmergencySupport" });

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 25
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                25 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 25);

            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 17, "FrontOfHouseAdmin" },
                    { 18, "BackOfficeAdmin" },
                    { 19, "HealthcareAssistant" },
                    { 20, "Steward" }
                });
        }
    }
}
