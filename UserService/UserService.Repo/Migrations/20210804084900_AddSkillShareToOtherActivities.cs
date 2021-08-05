using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class AddSkillShareToOtherActivities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[] { 32, "BankStaffVaccinator" });

            migrationBuilder.InsertData(
                schema: "Lookup",
                table: "SupportActivity",
                columns: new[] { "ID", "Name" },
                values: new object[] { 33, "SkillShare" });

            migrationBuilder.Sql(@"
                INSERT INTO [User].[SupportActivity]([UserID],[ActivityID])
                SELECT  UserID, 33
                FROM    [User].[SupportActivity] sa
                WHERE   ActivityID = 33 and 
		                33 not in (select ActivityID from [User].[SupportActivity] a where a.UserID = sa.UserID )
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 32);

            migrationBuilder.DeleteData(
                schema: "Lookup",
                table: "SupportActivity",
                keyColumn: "ID",
                keyValue: 33);
        }
    }
}
