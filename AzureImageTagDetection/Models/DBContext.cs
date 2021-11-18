using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AzureImageTagDetection.Models
{
    public partial class DBContext : DbContext
    {
        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AttributeEnumeration> AttributeEnumeration { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Opening> Opening { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Spare> Spare { get; set; }
        public virtual DbSet<SpareAttribute> SpareAttribute { get; set; }
        public virtual DbSet<SparePhoto> SparePhoto { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<StaffAttribute> StaffAttribute { get; set; }
        public virtual DbSet<StaffPhoto> StaffPhoto { get; set; }
        public virtual DbSet<Waitlist> Waitlist { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:dbinterview9.database.windows.net,1433;Database=dbInterview_2019;Connection Timeout=30;User ID=dbinterview9user;Password=Hxes-Ku2Eqc6)x8u;");

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AttributeEnumeration>(entity =>
            {
                entity.HasKey(e => e.AttributeKey);

                entity.Property(e => e.AttributeKey)
                    .HasMaxLength(10)
                    .ValueGeneratedNever();

                entity.Property(e => e.AttributeSet).HasMaxLength(100);
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.ExternalKey).HasMaxLength(125);

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Path).IsRequired();

                entity.Property(e => e.Tags)
                    .IsRequired()
                    .HasColumnName("tags");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Address2).HasMaxLength(1000);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.Phone1)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone2)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostalZip)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Province)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.AcknowledgementDate).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .HasColumnName("IP")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Latitude).HasColumnType("decimal(12, 9)");

                entity.Property(e => e.Longitude).HasColumnType("decimal(12, 9)");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.SentDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.AdminStaff)
                    .WithMany(p => p.NotificationAdminStaff)
                    .HasForeignKey(d => d.AdminStaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_Staff1");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.NotificationStaff)
                    .HasForeignKey(d => d.StaffId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notification_Staff");
            });

            modelBuilder.Entity<Opening>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.FillDate).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Schedule)
                    .WithMany(p => p.Opening)
                    .HasForeignKey(d => d.ScheduleId)
                    .HasConstraintName("FK_Opening_Schedule");

                entity.HasOne(d => d.Spare)
                    .WithMany(p => p.Opening)
                    .HasForeignKey(d => d.SpareId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Opening_Spare");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.Opening)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_Opening_Staff");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.Path).IsRequired();
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Schedule)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("FK_Schedule_Location");
            });

            modelBuilder.Entity<Spare>(entity =>
            {
                entity.HasKey(e => e.SpareId);

                entity.Property(e => e.Birthdate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SpareAttribute>(entity =>
            {
                entity.Property(e => e.AttributeKey)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.AttributeSet).HasMaxLength(100);

                entity.HasOne(d => d.Spare)
                    .WithMany(p => p.SpareAttribute)
                    .HasForeignKey(d => d.SpareId)
                    .HasConstraintName("FK_SpareAttribute_Spare");
            });

            modelBuilder.Entity<SparePhoto>(entity =>
            {
                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.SparePhoto)
                    .HasForeignKey(d => d.PhotoId)
                    .HasConstraintName("FK_SparePhoto_Photo");

                entity.HasOne(d => d.Spare)
                    .WithMany(p => p.SparePhoto)
                    .HasForeignKey(d => d.SpareId)
                    .HasConstraintName("FK_SparePhoto_Spare");
            });

            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId);

                entity.Property(e => e.Birthdate).HasColumnType("datetime");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Gender)
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasColumnName("IP")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TimeZone)
                    .HasMaxLength(5)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<StaffAttribute>(entity =>
            {
                entity.Property(e => e.AttributeKey)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.AttributeSet).HasMaxLength(100);

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffAttribute)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_StaffAttribute_Staff");
            });

            modelBuilder.Entity<StaffPhoto>(entity =>
            {
                entity.HasOne(d => d.Photo)
                    .WithMany(p => p.StaffPhoto)
                    .HasForeignKey(d => d.PhotoId)
                    .HasConstraintName("FK_StaffPhoto_Photo");

                entity.HasOne(d => d.Staff)
                    .WithMany(p => p.StaffPhoto)
                    .HasForeignKey(d => d.StaffId)
                    .HasConstraintName("FK_StaffPhoto_Staff");
            });

            modelBuilder.Entity<Waitlist>(entity =>
            {
                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Opening)
                    .WithMany(p => p.Waitlist)
                    .HasForeignKey(d => d.OpeningId)
                    .HasConstraintName("FK_Waitlist_Opening");

                entity.HasOne(d => d.Spare)
                    .WithMany(p => p.Waitlist)
                    .HasForeignKey(d => d.SpareId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Waitlist_Spare");
            });
        }

        internal void SaveChanges(Image img)
        {
            throw new NotImplementedException();
        }
    }
}
