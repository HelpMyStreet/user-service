using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.Repo.Migrations
{
    public partial class SqlCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cache");

            migrationBuilder.EnsureSchema(
                name: "Precalculation");

            migrationBuilder.CreateTable(
                name: "Data",
                schema: "Cache",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "MetaData",
                schema: "Precalculation",
                columns: table => new
                {
                    TableName = table.Column<string>(unicode: false, maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaData", x => x.TableName);
                });

            migrationBuilder.CreateTable(
                name: "Volunteer",
                schema: "Precalculation",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Postcode = table.Column<string>(unicode: false, maxLength: 10, nullable: true),
                    SupportRadiusMiles = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    VolunteerType = table.Column<byte>(nullable: false),
                    IsVerifiedType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Volunteer", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Volunteer_Latitude_Longitude_VolunteerType_IsVerifiedType",
                schema: "Precalculation",
                table: "Volunteer",
                columns: new[] { "Latitude", "Longitude", "VolunteerType", "IsVerifiedType" })
                .Annotation("SqlServer:Include", new[] { "UserId", "Postcode", "SupportRadiusMiles" });

            migrationBuilder.Sql(@"
CREATE TYPE [PreCalculation].[Volunteer] AS TABLE(
	[UserId] [int]  NOT NULL,
	[Postcode] [varchar](10) NULL,
	[SupportRadiusMiles] [float] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[VolunteerType] [tinyint] NOT NULL,
	[IsVerifiedType] [tinyint] NOT NULL
) 
  ");

            migrationBuilder.Sql(@"
CREATE PROC [PreCalculation].[AddOrUpdateVolunteer]
@volunteers [PreCalculation].[Volunteer] READONLY
AS 
BEGIN
  SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;  
	SET XACT_ABORT ON
    
	BEGIN TRY
	BEGIN TRANSACTION

	TRUNCATE TABLE [PreCalculation].[Volunteer]

	INSERT INTO [PreCalculation].[Volunteer]
	([UserId]
      ,[Postcode]
      ,[SupportRadiusMiles]
      ,[Latitude]
      ,[Longitude]
      ,[VolunteerType]
      ,[IsVerifiedType])

	SELECT [UserId]
      ,[Postcode]
      ,[SupportRadiusMiles]
      ,[Latitude]
      ,[Longitude]
      ,[VolunteerType]
      ,[IsVerifiedType]
	  FROM @volunteers
	
	UPDATE [PreCalculation].[MetaData]
	SET [LastUpdated] = GETUTCDATE()
	WHERE [TableName] = 'Volunteer'

	COMMIT
	END TRY

	BEGIN CATCH
		IF @@trancount > 0
			ROLLBACK ;
			THROW;
	END CATCH
END

  ");

            migrationBuilder.Sql(@"
CREATE PROC [Precalculation].[GetVolunteersInBoundary]
@swLatitude FLOAT,
@neLatitude FLOAT,
@swLongitude FLOAT,
@neLongitude FLOAT,
@volunteerType TINYINT,
@isVerifiedType TINYINT
AS 
BEGIN

	SELECT [UserId]
      ,[Postcode]
      ,[SupportRadiusMiles]
      ,[Latitude]
      ,[Longitude]
      ,[VolunteerType]
      ,[IsVerifiedType]
	  FROM [PreCalculation].[Volunteer]
	  WHERE [Latitude] >= @swLatitude
	  AND [Latitude] <= @neLatitude
	  AND [Longitude] >= @swLongitude
	  AND [Longitude] <= @neLongitude
	  AND [VolunteerType] = @volunteerType 
	  AND [IsVerifiedType] = @isVerifiedType

END
  ");

            migrationBuilder.Sql(@"
CREATE PROC [Cache].[GetData]
@key VARCHAR(50) 
AS
SELECT [Key]
      ,[LastUpdated]
      ,[Data]
  FROM [Cache].[Data]
  WHERE [Key] = @key
  ");


            migrationBuilder.Sql(@"
CREATE PROC [Cache].[AddOrUpdateData]
@key VARCHAR(50),
@data BINARY 
AS
BEGIN
	SET XACT_ABORT ON	

	BEGIN TRY
  BEGIN TRANSACTION

	IF EXISTS (SELECT TOP 1 [Key] FROM [Cache].[Data] WHERE [Key] = @key)
	BEGIN

	UPDATE [Cache].[Data]
	SET [LastUpdated] = GETUTCDATE(), 
		[Data] = @data
	WHERE [Key] = @key

	END
	ELSE
	BEGIN

	INSERT INTO [Cache].[Data]
	([Key]
      ,[LastUpdated]
      ,[Data])
	VALUES(@key, GETUTCDATE(), @data)
	END
	
	COMMIT
	END TRY

	BEGIN CATCH
		IF @@trancount > 0
			ROLLBACK;
			THROW;
	END CATCH
END
  ");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Data",
                schema: "Cache");

            migrationBuilder.DropTable(
                name: "MetaData",
                schema: "Precalculation");

            migrationBuilder.DropTable(
                name: "Volunteer",
                schema: "Precalculation");



            migrationBuilder.Sql(@"
DROP TYPE [PreCalculation].[Volunteer]
  ");

            migrationBuilder.Sql(@"
DROP PROC [PreCalculation].[AddOrUpdateVolunteer]
  ");

            migrationBuilder.Sql(@"
DROP PROC [Precalculation].[GetVolunteersInBoundary]
  ");

            migrationBuilder.Sql(@"
DROP PROC [Cache].[GetData]
  ");

            migrationBuilder.Sql(@"
DROP PROC [Cache].[AddOrUpdateData]
  ");
        }
    }
}
