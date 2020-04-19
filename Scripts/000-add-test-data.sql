DECLARE @numberOfTestCases INT = 200000
DECLARE @numberOfStreetChampions INT = 10000

DELETE
FROM [UserService].[User].[SupportActivity]

DELETE
FROM [UserService].[User].[ChampionPostcode]

DELETE
FROM [UserService].[UserPersonal].[PersonalDetails]

DELETE
FROM [UserService].[User].[User]

DBCC CHECKIDENT (
		'[UserService].[User].[User]',
		RESEED,
		0
		);

DECLARE @numberOfPostcodes INT = (
		SELECT count(1)
		FROM [AddressService].[Address].[Postcode]
		)
DECLARE @everyXthPostcode INT = @numberOfPostcodes / @numberOfTestCases


INSERT INTO [UserService].[User].[User] (
	[FirebaseUID],
	[PostalCode],
	[EmailSharingConsent],
	[MobileSharingConsent],
	[OtherPhoneSharingConsent],
	[HMSContactConsent],
	[IsVolunteer],
	[IsVerified],
	[DateCreated],
	[SupportRadiusMiles],
	[SupportVolunteersByPhone],
	[StreetChampionRoleUnderstood]
	)
SELECT TOP (@numberOfTestCases) NewId(),
	[Postcode],
	1,
	1,
	1,
	1,
	1,
	1,
	GetUtcDate(),
	CONVERT(FLOAT, 0.5 + (3 - 0.5) * RAND(CHECKSUM(NEWID()))),
	1,
	1
FROM [AddressService].[Address].[Postcode]
WHERE [Id] % @everyXthPostcode = 0

INSERT INTO [UserService].[UserPersonal].[PersonalDetails] (
	[UserID],
	[FirstName],
	[LastName],
	[DisplayName],
	[DateOfBirth],
	[AddressLine1],
	[AddressLine2],
	[AddressLine3],
	[Locality],
	[Postcode],
	[EmailAddress],
	[MobilePhone],
	[OtherPhone],
	[UnderlyingMedicalCondition]
	)
SELECT u.[ID],
	'firstName',
	'lastName',
	'displayName',
	'1980-01-01',
	'AddressLine1',
	'AddressLine2',
	'AddressLine3',
	'Locality',
	u.[PostalCode],
	'email@addresss',
	'07854 745157',
	'16494151651',
	0
FROM [UserService].[User].[User] u
WHERE NOT EXISTS (
		SELECT *
		FROM [UserService].[UserPersonal].[PersonalDetails] pd
		WHERE pd.UserID = u.ID
		)

INSERT INTO [UserService].[User].[SupportActivity] (
	[UserID],
	[ActivityID]
	)
SELECT [ID],
	floor(RAND(CHECKSUM(NEWID())) * (11) + 1)
FROM [UserService].[User].[User]


DECLARE @numberOfUsers INT = (
		SELECT count(*)
		FROM [UserService].[User].[User]
		)
DECLARE @everyXthUserIsAStreetChamp INT = @numberOfUsers / @numberOfStreetChampions

UPDATE [UserService].[User].[User]
SET [StreetChampionRoleUnderstood] = 1
WHERE [ID] % @everyXthUserIsAStreetChamp = 0

INSERT INTO [UserService].[User].[ChampionPostcode] (
	[UserID],
	[PostalCode]
	)
SELECT [ID],
	[PostalCode]
FROM [UserService].[User].[User]
WHERE [ID] % @everyXthUserIsAStreetChamp = 0