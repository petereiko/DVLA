using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WinApp.Data
{
    public partial class DVLADBContext : DbContext
    {
        public DVLADBContext()
            : base("name=DVLADBContext")
        {
        }

        public virtual DbSet<C__EFMigrationsHistory> C__EFMigrationsHistory { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<Applicant> Applicants { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<ColourVisionScore> ColourVisionScores { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<EmailLogAttachment> EmailLogAttachments { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailTemplateToken> EmailTemplateTokens { get; set; }
        public virtual DbSet<EmailToken> EmailTokens { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<FormNumber> FormNumbers { get; set; }
        public virtual DbSet<InitiatePaystackTransferRequest> InitiatePaystackTransferRequests { get; set; }
        public virtual DbSet<InitiatePaystackTransferRespons> InitiatePaystackTransferResponses { get; set; }
        public virtual DbSet<MessageBox> MessageBoxes { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<ModuleAction> ModuleActions { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<OptometristFirm> OptometristFirms { get; set; }
        public virtual DbSet<OptometristFirmUser> OptometristFirmUsers { get; set; }
        public virtual DbSet<PaystackVerification> PaystackVerifications { get; set; }
        public virtual DbSet<QueryBuilder> QueryBuilders { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }
        public virtual DbSet<SerialNumber> SerialNumbers { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<SlotPrice> SlotPrices { get; set; }
        public virtual DbSet<SlotReductionLog> SlotReductionLogs { get; set; }
        public virtual DbSet<SlotReOrderLevel> SlotReOrderLevels { get; set; }
        public virtual DbSet<SlotRequest> SlotRequests { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<SmsLog> SmsLogs { get; set; }
        public virtual DbSet<SmsTemplate> SmsTemplates { get; set; }
        public virtual DbSet<SmsTemplateToken> SmsTemplateTokens { get; set; }
        public virtual DbSet<SmsToken> SmsTokens { get; set; }
        public virtual DbSet<VisualAcuityScore> VisualAcuityScores { get; set; }
        public virtual DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }
        public virtual DbSet<VisualFieldScore> VisualFieldScores { get; set; }
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Hash> Hashes { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobParameter> JobParameters { get; set; }
        public virtual DbSet<JobQueue> JobQueues { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<Schema> Schemata { get; set; }
        public virtual DbSet<Server> Servers { get; set; }
        public virtual DbSet<Set> Sets { get; set; }
        public virtual DbSet<State> States { get; set; }
        public virtual DbSet<SystemAdmin> SystemAdmins { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActivityLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Applicant>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetRoleClaims)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetRole)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserTokens)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AuditLogs)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.ApplicationUserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.OptometristFirmUsers)
                .WithOptional(e => e.AspNetUser)
                .HasForeignKey(e => e.ApplicationUserId);

            modelBuilder.Entity<AuditLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ColourVisionScore>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailLogAttachment>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailTemplate>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailTemplateToken>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailToken>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<InitiatePaystackTransferRequest>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<InitiatePaystackTransferRespons>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<MessageBox>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Message>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ModuleAction>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Module>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Module>()
                .HasMany(e => e.ModuleActions)
                .WithOptional(e => e.Module)
                .HasForeignKey(e => e.ModuleId1);

            modelBuilder.Entity<OptometristFirm>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<OptometristFirmUser>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<PaystackVerification>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<QueryBuilder>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Reminder>()
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

            modelBuilder.Entity<SmsLog>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SmsTemplate>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SmsTemplateToken>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SmsToken>()
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
