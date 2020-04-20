drop table [User].[RegistrationHistory]

create table [User].[RegistrationHistory]
(
    UserID              int not null,
    RegistrationStep    tinyint not null,
    DateCompleted       datetime not null,
    CONSTRAINT PK_RegistrationHistory PRIMARY KEY (UserID, RegistrationStep),
    CONSTRAINT FK_RegistrationHistory_User FOREIGN KEY (UserID) REFERENCES [User].[User] (ID)
)