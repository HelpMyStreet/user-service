﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserService.Repo;

namespace UserService.Repo.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HelpMyStreet.PostcodeCoordinates.EF.Entities.PostcodeEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("LastUpdated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Postcode")
                        .IsUnique()
                        .HasName("UX_Postcode_Postcode");

                    b.HasIndex("Postcode", "IsActive")
                        .HasName("IX_Postcode_Postcode_IsActive")
                        .HasAnnotation("SqlServer:Include", new[] { "Latitude", "Longitude" });

                    b.HasIndex("Latitude", "Longitude", "IsActive")
                        .HasName("IX_Postcode_Latitude_Longitude_IsActive")
                        .HasAnnotation("SqlServer:Include", new[] { "Postcode" });

                    b.ToTable("Postcode","Address");
                });

            modelBuilder.Entity("HelpMyStreet.PostcodeCoordinates.EF.Entities.PostcodeEntityOldEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("LastUpdated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Postcode")
                        .IsUnique()
                        .HasName("UX_Postcode_Postcode");

                    b.HasIndex("Postcode", "IsActive")
                        .HasName("IX_Postcode_Postcode_IsActive")
                        .HasAnnotation("SqlServer:Include", new[] { "Latitude", "Longitude" });

                    b.HasIndex("Latitude", "Longitude", "IsActive")
                        .HasName("IX_Postcode_Latitude_Longitude_IsActive")
                        .HasAnnotation("SqlServer:Include", new[] { "Postcode" });

                    b.ToTable("Postcode_Old","Staging");
                });

            modelBuilder.Entity("HelpMyStreet.PostcodeCoordinates.EF.Entities.PostcodeEntitySwitchEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<DateTime>("LastUpdated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2(0)")
                        .HasDefaultValueSql("GetUtcDate()");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.HasIndex("Postcode")
                        .IsUnique()
                        .HasName("UX_Postcode_Postcode");

                    b.HasIndex("Postcode", "IsActive")
                        .HasName("IX_Postcode_Postcode_IsActive")
                        .HasAnnotation("SqlServer:Include", new[] { "Latitude", "Longitude" });

                    b.HasIndex("Latitude", "Longitude", "IsActive")
                        .HasName("IX_Postcode_Latitude_Longitude_IsActive")
                        .HasAnnotation("SqlServer:Include", new[] { "Postcode" });

                    b.ToTable("Postcode_Switch","Staging");
                });

            modelBuilder.Entity("HelpMyStreet.PostcodeCoordinates.EF.Entities.PostcodeStagingEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(9,6)");

                    b.Property<string>("Postcode")
                        .IsRequired()
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("Id");

                    b.ToTable("Postcode_Staging","Staging");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.Biography", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Details")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "DateCreated");

                    b.ToTable("Biography","UserPersonal");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.ChampionPostcode", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.Property<string>("PostalCode")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("UserId", "PostalCode");

                    b.HasIndex("UserId", "PostalCode")
                        .HasName("ix_PostalCode");

                    b.ToTable("ChampionPostcode","User");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.EnumSupportActivities", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SupportActivity","Lookup");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Shopping"
                        },
                        new
                        {
                            Id = 2,
                            Name = "CollectingPrescriptions"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Errands"
                        },
                        new
                        {
                            Id = 4,
                            Name = "MedicalAppointmentTransport"
                        },
                        new
                        {
                            Id = 5,
                            Name = "DogWalking"
                        },
                        new
                        {
                            Id = 6,
                            Name = "MealPreparation"
                        },
                        new
                        {
                            Id = 7,
                            Name = "PhoneCalls_Friendly"
                        },
                        new
                        {
                            Id = 8,
                            Name = "PhoneCalls_Anxious"
                        },
                        new
                        {
                            Id = 9,
                            Name = "HomeworkSupport"
                        },
                        new
                        {
                            Id = 10,
                            Name = "CheckingIn"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Other"
                        },
                        new
                        {
                            Id = 12,
                            Name = "FaceMask"
                        },
                        new
                        {
                            Id = 13,
                            Name = "WellbeingPackage"
                        },
                        new
                        {
                            Id = 14,
                            Name = "CommunityConnector"
                        },
                        new
                        {
                            Id = 15,
                            Name = "ColdWeatherArmy"
                        },
                        new
                        {
                            Id = 16,
                            Name = "Transport"
                        },
                        new
                        {
                            Id = 21,
                            Name = "MealsToYourDoor"
                        },
                        new
                        {
                            Id = 22,
                            Name = "VolunteerSupport"
                        },
                        new
                        {
                            Id = 23,
                            Name = "MealtimeCompanion"
                        },
                        new
                        {
                            Id = 24,
                            Name = "VaccineSupport"
                        },
                        new
                        {
                            Id = 25,
                            Name = "EmergencySupport"
                        },
                        new
                        {
                            Id = 26,
                            Name = "InPersonBefriending"
                        },
                        new
                        {
                            Id = 27,
                            Name = "PracticalSupport"
                        },
                        new
                        {
                            Id = 28,
                            Name = "VolunteerInduction"
                        },
                        new
                        {
                            Id = 29,
                            Name = "DigitalSupport"
                        },
                        new
                        {
                            Id = 30,
                            Name = "BinDayAssistance"
                        },
                        new
                        {
                            Id = 31,
                            Name = "Covid19Help"
                        },
                        new
                        {
                            Id = 32,
                            Name = "BankStaffVaccinator"
                        },
                        new
                        {
                            Id = 33,
                            Name = "SkillShare"
                        },
                        new
                        {
                            Id = 34,
                            Name = "Accommodation"
                        },
                        new
                        {
                            Id = 35,
                            Name = "AdvertisingRoles"
                        },
                        new
                        {
                            Id = 36,
                            Name = "NHSSteward"
                        },
                        new
                        {
                            Id = 37,
                            Name = "NHSTransport"
                        },
                        new
                        {
                            Id = 38,
                            Name = "NHSCheckInAndChat"
                        },
                        new
                        {
                            Id = 39,
                            Name = "NHSCheckInAndChatPlus"
                        });
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.PersonalDetails", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.Property<string>("AddressLine1")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine2")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("AddressLine3")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("DisplayName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("EmailAddress")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("FirstName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("LastName")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<string>("Locality")
                        .HasColumnType("varchar(100)")
                        .HasMaxLength(100)
                        .IsUnicode(false);

                    b.Property<string>("MobilePhone")
                        .HasColumnType("varchar(15)")
                        .HasMaxLength(15)
                        .IsUnicode(false);

                    b.Property<string>("OtherPhone")
                        .HasColumnType("varchar(15)")
                        .HasMaxLength(15)
                        .IsUnicode(false);

                    b.Property<string>("Postcode")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<bool?>("UnderlyingMedicalCondition")
                        .HasColumnType("bit");

                    b.HasKey("UserId");

                    b.HasIndex("EmailAddress")
                        .IsUnique()
                        .HasName("UC_EmailAddress")
                        .HasFilter("[EmailAddress] IS NOT NULL");

                    b.ToTable("PersonalDetails","UserPersonal");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.RegistrationHistory", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.Property<byte>("RegistrationStep")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("DateCompleted")
                        .HasColumnType("datetime");

                    b.HasKey("UserId", "RegistrationStep");

                    b.ToTable("RegistrationHistory","User");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.SupportActivity", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.Property<byte>("ActivityId")
                        .HasColumnName("ActivityID")
                        .HasColumnType("tinyint");

                    b.HasKey("UserId", "ActivityId");

                    b.ToTable("SupportActivity","User");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.SupportPostcode", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnName("UserID")
                        .HasColumnType("int");

                    b.Property<string>("PostalCode")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.HasKey("UserId", "PostalCode");

                    b.HasIndex("UserId", "PostalCode")
                        .HasName("ix_PostalCode");

                    b.ToTable("SupportPostcode","User");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("DateLastLogin")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateLastLoginChecked")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("EmailSharingConsent")
                        .HasColumnType("bit");

                    b.Property<string>("FirebaseUid")
                        .IsRequired()
                        .HasColumnName("FirebaseUID")
                        .HasColumnType("varchar(50)")
                        .HasMaxLength(50)
                        .IsUnicode(false);

                    b.Property<bool?>("HmscontactConsent")
                        .HasColumnName("HMSContactConsent")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsVolunteer")
                        .HasColumnType("bit");

                    b.Property<bool?>("MobileSharingConsent")
                        .HasColumnType("bit");

                    b.Property<bool?>("OtherPhoneSharingConsent")
                        .HasColumnType("bit");

                    b.Property<string>("PostalCode")
                        .HasColumnType("varchar(10)")
                        .HasMaxLength(10)
                        .IsUnicode(false);

                    b.Property<int?>("ReferringGroupId")
                        .HasColumnType("int");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("StreetChampionRoleUnderstood")
                        .HasColumnType("bit");

                    b.Property<double?>("SupportRadiusMiles")
                        .HasColumnType("float");

                    b.Property<bool?>("SupportVolunteersByPhone")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("FirebaseUid")
                        .IsUnique()
                        .HasName("UC_FirebaseUID");

                    b.ToTable("User","User");
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.Biography", b =>
                {
                    b.HasOne("UserService.Repo.EntityFramework.Entities.User", "User")
                        .WithMany("Biography")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_Biography_User")
                        .IsRequired();
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.ChampionPostcode", b =>
                {
                    b.HasOne("UserService.Repo.EntityFramework.Entities.User", "User")
                        .WithMany("ChampionPostcode")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_ChampionPostcode_User")
                        .IsRequired();
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.PersonalDetails", b =>
                {
                    b.HasOne("UserService.Repo.EntityFramework.Entities.User", "User")
                        .WithOne("PersonalDetails")
                        .HasForeignKey("UserService.Repo.EntityFramework.Entities.PersonalDetails", "UserId")
                        .HasConstraintName("FK_PersonalDetails_User")
                        .IsRequired();
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.RegistrationHistory", b =>
                {
                    b.HasOne("UserService.Repo.EntityFramework.Entities.User", "User")
                        .WithMany("RegistrationHistory")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_RegistrationHistory_User")
                        .IsRequired();
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.SupportActivity", b =>
                {
                    b.HasOne("UserService.Repo.EntityFramework.Entities.User", "User")
                        .WithMany("SupportActivity")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_SupportActivity_User")
                        .IsRequired();
                });

            modelBuilder.Entity("UserService.Repo.EntityFramework.Entities.SupportPostcode", b =>
                {
                    b.HasOne("UserService.Repo.EntityFramework.Entities.User", "User")
                        .WithMany("SupportPostcode")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_SupportPostcode_User")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
