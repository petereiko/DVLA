using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DVLA.WindowsApplication.Data
{
    public partial class DVLAContext : DbContext
    {
        public DVLAContext()
            : base("name=DefaultConnection")
        {
        }

        public virtual DbSet<Applicant> Applicants { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<ColourVisionScore> ColourVisionScores { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<OptometristFirm> OptometristFirms { get; set; }
        public virtual DbSet<OptometristFirmUser> OptometristFirmUsers { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<SerialNumber> SerialNumbers { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<SlotPrice> SlotPrices { get; set; }
        public virtual DbSet<SlotReductionLog> SlotReductionLogs { get; set; }
        public virtual DbSet<SlotReOrderLevel> SlotReOrderLevels { get; set; }
        public virtual DbSet<SlotRequest> SlotRequests { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<SystemAdmin> SystemAdmins { get; set; }
        public virtual DbSet<VisualAcuityScore> VisualAcuityScores { get; set; }
        public virtual DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }
        public virtual DbSet<VisualFieldScore> VisualFieldScores { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Applicant>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUser>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.OptometristFirmUsers)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.ApplicationUserId);

            modelBuilder.Entity<ColourVisionScore>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<OptometristFirm>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<OptometristFirmUser>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SerialNumber>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ServiceType>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SlotPrice>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SlotReductionLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SlotReOrderLevel>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SlotRequest>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Slot>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<VisualAcuityScore>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<VisualAssessmentResult>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<VisualFieldScore>()
                .Property(e => e.RowVersion)
                .IsFixedLength();
        }
    }
}
