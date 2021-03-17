using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddBinAssistanceToOtherActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 26, "InPersonBefriending" },
                    { 27, "PracticalSupport" },
                    { 28, "VolunteerInduction" },
                    { 29, "DigitalSupport" },
                    { 30, "BinDayAssistance" },
                    { 31, "Covid19Help" }
                });

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 26
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                26 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 27
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                27 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 28
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                28 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 29
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                29 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 30
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                30 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 31
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 11 and 
		                31 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 26);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 27);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 28);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 29);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 30);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 31);
        }
    }
}
