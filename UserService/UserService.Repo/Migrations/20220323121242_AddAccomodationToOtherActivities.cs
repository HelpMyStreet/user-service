using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddAccomodationToOtherActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[] { 34, "Accommodation" });

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 34
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 34 and 
		                34 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 34);
        }
    }
}
