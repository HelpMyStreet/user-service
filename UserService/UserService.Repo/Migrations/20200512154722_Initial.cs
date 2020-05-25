using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "User");

            migrationBuilder.EnsureSchema(
                name: "UserPersonal");

            migrationBuilder.CreateTable(
                name: "User",
                schema: "User",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirebaseUID = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    PostalCode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    EmailSharingConsent = table.Column<bool>(nullable: true),
                    MobileSharingConsent = table.Column<bool>(nullable: true),
                    OtherPhoneSharingConsent = table.Column<bool>(nullable: true),
                    HMSContactConsent = table.Column<bool>(nullable: true),
                    IsVolunteer = table.Column<bool>(nullable: true),
                    IsVerified = table.Column<bool>(nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime", nullable: true),
                    SupportRadiusMiles = table.Column<double>(nullable: true),
                    SupportVolunteersByPhone = table.Column<bool>(nullable: true),
                    StreetChampionRoleUnderstood = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ChampionPostcode",
                schema: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChampionPostcode", x => new { x.UserID, x.PostalCode });
                    table.ForeignKey(
                        name: "FK_ChampionPostcode_User",
                        column: x => x.UserID,
                        principalSchema: "User",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegistrationHistory",
                schema: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    RegistrationStep = table.Column<byte>(nullable: false),
                    DateCompleted = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrationHistory", x => new { x.UserID, x.RegistrationStep });
                    table.ForeignKey(
                        name: "FK_RegistrationHistory_User",
                        column: x => x.UserID,
                        principalSchema: "User",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportActivity",
                schema: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    ActivityID = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportActivity", x => new { x.UserID, x.ActivityID });
                    table.ForeignKey(
                        name: "FK_SupportActivity_User",
                        column: x => x.UserID,
                        principalSchema: "User",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportPostcode",
                schema: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportPostcode", x => new { x.UserID, x.PostalCode });
                    table.ForeignKey(
                        name: "FK_SupportPostcode_User",
                        column: x => x.UserID,
                        principalSchema: "User",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalDetails",
                schema: "UserPersonal",
                columns: table => new
                {
                    UserID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    LastName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    DisplayName = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: true),
                    AddressLine1 = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    AddressLine2 = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    AddressLine3 = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Locality = table.Column<string>(unicode: false, maxLength: 100, nullable: true),
                    Postcode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    EmailAddress = table.Column<string>(unicode: false, maxLength: 50, nullable: true),
                    MobilePhone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    OtherPhone = table.Column<string>(unicode: false, maxLength: 15, nullable: true),
                    UnderlyingMedicalCondition = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDetails", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_PersonalDetails_User",
                        column: x => x.UserID,
                        principalSchema: "User",
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_PostalCode",
                schema: "User",
                table: "ChampionPostcode",
                columns: new[] { "UserID", "PostalCode" });

            migrationBuilder.CreateIndex(
                name: "ix_PostalCode",
                schema: "User",
                table: "SupportPostcode",
                columns: new[] { "UserID", "PostalCode" });

            migrationBuilder.CreateIndex(
                name: "ix_FirebaseUID",
                schema: "User",
                table: "User",
                column: "FirebaseUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChampionPostcode",
                schema: "User");

            migrationBuilder.DropTable(
                name: "RegistrationHistory",
                schema: "User");

            migrationBuilder.DropTable(
                name: "SupportActivity",
                schema: "User");

            migrationBuilder.DropTable(
                name: "SupportPostcode",
                schema: "User");

            migrationBuilder.DropTable(
                name: "PersonalDetails",
                schema: "UserPersonal");

            migrationBuilder.DropTable(
                name: "User",
                schema: "User");
        }
    }
}
