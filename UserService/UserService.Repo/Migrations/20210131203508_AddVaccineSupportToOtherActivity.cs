using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddVaccineSupportToOtherActivity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 15, "ColdWeatherArmy" },
                    { 16, "Transport" },
                    { 17, "FrontOfHouseAdmin" },
                    { 18, "BackOfficeAdmin" },
                    { 19, "HealthcareAssistant" },
                    { 20, "Steward" },
                    { 21, "MealsToYourDoor" },
                    { 22, "VolunteerSupport" },
                    { 23, "MealtimeCompanion" },
                    { 24, "VaccineSupport" }
                });

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 24
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                24 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 15);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 16);

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

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 21);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 22);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 24);
        }
    }
}
