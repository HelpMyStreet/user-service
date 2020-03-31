
---
drop table [User].[SupportActivity]
drop table [User].[SupportPostcode]
drop table [UserPersonal].[PersonalDetails]
drop table [User].[User]
drop schema [UserPersonal]
drop schema [User]
---

CREATE SCHEMA [User]
CREATE SCHEMA [UserPersonal]

CREATE TABLE [User].[User]
(
    ID                          int identity(1,1) not null,
    FirebaseUID                 varchar(50) not null,
    PostalCode                  varchar(10) null,
    EmailSharingConsent	        bit null,
    MobileSharingConsent	    bit null,
    OtherPhoneSharingConsent	bit null,
    HMSContactConsent	        bit null,
    IsVolunteer		            bit null,
    IsVerified		            bit null,
    DateCreated		            datetime null,
    CONSTRAINT PK_User PRIMARY KEY (ID)
)

CREATE INDEX ix_FirebaseUID ON [User].[User] (FirebaseUID)



CREATE TABLE [UserPersonal].[PersonalDetails]
(
    ID	            int identity(1,1) not null,
    UserID          int not null, -- Historically I'd have made this the primary key and removed the identity column; not sure how that would play with entity framework.  I'm not precious either way.
    FirstName	    varchar(50) null,
    LastName	    varchar(50) null,
    DisplayName	    varchar(50) null,
    DateOfBirth	    date null,
    AddressLine1	varchar(100) null,
    AddressLine2	varchar(100) null,
    AddressLine3	varchar(100) null,
    Locality	    varchar(100) null,
    Postcode	    varchar(10) null,
    EmailAddress	varchar(50) null,
    MobilePhone	    varchar(15) null,
    OtherPhone	    varchar(15) null,
    CONSTRAINT PK_PersonalDetails PRIMARY KEY (ID),
    CONSTRAINT FK_PersonalDetails_User FOREIGN KEY (UserID) REFERENCES [User].[User] (ID)
)

CREATE INDEX ix_UserID ON [UserPersonal].[PersonalDetails] (UserID)



CREATE TABLE [User].[SupportPostcode]
(
    -- Do we need an identity column here too for the primary key?  I'd prefer not, but speed is of the essence...
    UserID      int not null,
    PostalCode  varchar(10) not null,
    CONSTRAINT PK_SupportPostcode PRIMARY KEY (UserID, PostalCode),
    CONSTRAINT FK_SupportPostcode_User FOREIGN KEY (UserID) REFERENCES [User].[User] (ID)
)

CREATE INDEX ix_PostalCode ON [User].[SupportPostcode] (PostalCode) INCLUDE (UserID)
-- Add the following index if the primary key changes
--CREATE INDEX ix_UserID ON [User].[SupportPostcode] (UserID) INCLUDE (PostalCode)



CREATE TABLE [User].[ChampionPostcode]
(
    -- Do we need an identity column here too for the primary key?  I'd prefer not, but speed is of the essence...
    UserID      int not null,
    PostalCode  varchar(10) not null,
    CONSTRAINT PK_ChampionPostcode PRIMARY KEY (UserID, PostalCode),
    CONSTRAINT FK_ChampionPostcode_User FOREIGN KEY (UserID) REFERENCES [User].[User] (ID)
)

CREATE INDEX ix_PostalCode ON [User].[ChampionPostcode] (PostalCode) INCLUDE (UserID)
-- Add the following index if the primary key changes
--CREATE INDEX ix_UserID ON [User].[ChampionPostcode] (UserID) INCLUDE (PostalCode)



CREATE TABLE [User].[SupportActivity]
(
    -- Do we need an identity column here too for the primary key?  I'd prefer not, but speed is of the essence...
    UserID          int not null, 
    ActivityID      tinyint not null,
    CONSTRAINT PK_SupportActivity PRIMARY KEY (UserID, ActivityID),
    CONSTRAINT FK_SupportActivity_User FOREIGN KEY (UserID) REFERENCES [User].[User] (UserID)
)

-- Add the following index if the primary key changes
--CREATE INDEX ix_UserID ON [User].[SupportActivity] (UserID) INCLUDE (ActivityID)
