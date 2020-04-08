using UserService.Repo.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UserService.Repo
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChampionPostcode> ChampionPostcode { get; set; }
        public virtual DbSet<PersonalDetails> PersonalDetails { get; set; }
        public virtual DbSet<SupportActivity> SupportActivity { get; set; }
        public virtual DbSet<SupportPostcode> SupportPostcode { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChampionPostcode>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PostalCode });

                entity.ToTable("ChampionPostcode", "User");

                entity.HasIndex(e => new { e.UserId, e.PostalCode })
                    .HasName("ix_PostalCode");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChampionPostcode)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChampionPostcode_User");
            });

            modelBuilder.Entity<PersonalDetails>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("PersonalDetails", "UserPersonal");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AddressLine1)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine2)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AddressLine3)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DisplayName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmailAddress)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Locality)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MobilePhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.OtherPhone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Postcode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.PersonalDetails)
                    .HasForeignKey<PersonalDetails>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PersonalDetails_User");
            });

            modelBuilder.Entity<SupportActivity>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ActivityId });

                entity.ToTable("SupportActivity", "User");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SupportActivity)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupportActivity_User");
            });

            modelBuilder.Entity<SupportPostcode>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PostalCode });

                entity.ToTable("SupportPostcode", "User");

                entity.HasIndex(e => new { e.UserId, e.PostalCode })
                    .HasName("ix_PostalCode");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.SupportPostcode)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupportPostcode_User");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "User");

                entity.HasIndex(e => e.FirebaseUid)
                    .HasName("ix_FirebaseUID");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.FirebaseUid)
                    .IsRequired()
                    .HasColumnName("FirebaseUID")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HmscontactConsent).HasColumnName("HMSContactConsent");

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);
            });
        }
    }
}
