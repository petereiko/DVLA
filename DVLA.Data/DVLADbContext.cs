using DVLA.Data.Models.Auth;
using DVLA.DATA.Domains;
using DVLA.Data.Models.Domains;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLA.Data
{
    public class DVLADbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DVLADbContext(DbContextOptions<DVLADbContext> options) : base(options)
        {
        }

        #region MyDBSetRegion

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<SerialNumber> SerialNumbers { get; set; }
        public DbSet<FormNumber> FormNumbers { get; set; }
        public DbSet<ColourVisionScore> ColourVisionScores { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; }
        public virtual DbSet<EmailLogAttachment> EmailLogAttachments { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<EmailTemplateToken> EmailTemplateTokens { get; set; }
        public virtual DbSet<EmailToken> EmailTokens { get; set; }
        public virtual DbSet<InitiatePaystackTransferRequest> InitiatePaystackTransferRequests { get; set; }
        public virtual DbSet<InitiatePaystackTransferResponse> InitiatePaystackTransferResponses { get; set; }
        public virtual DbSet<SmsTemplate> SmsTemplates { get; set; }
        public virtual DbSet<SmsTemplateToken> SmsTemplateTokens { get; set; }
        public virtual DbSet<SmsToken> SmsTokens { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<MessageBox> MessageBoxes { get; set; }
        public virtual DbSet<OptometristFirm> OptometristFirms { get; set; }
        public virtual DbSet<QueryBuilder> QueryBuilders { get; set; } 
        public virtual DbSet<OptometristFirmUser> OptometristFirmUsers { get; set; }
        public virtual DbSet<PaystackVerification> PaystackVerifications { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<ModuleAction> ModuleActions { get; set; }
        public virtual DbSet<SmsLog> SmsLogs { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<SlotPrice> SlotPrices { get; set; }
        public virtual DbSet<SlotReductionLog> SlotReductionLogs { get; set; }
        public virtual DbSet<SlotReOrderLevel> SlotReOrderLevels { get; set; }
        public virtual DbSet<SlotRequest> SlotRequests { get; set; }
        public virtual DbSet<Slot> Slots { get; set; }
        public virtual DbSet<Module> Modules { get; set; }
        public virtual DbSet<Reminder> Reminders { get; set; }

        public virtual DbSet<VisualAcuityScore> VisualAcuityScores { get; set; }
        public virtual DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }
        public virtual DbSet<VisualAssessmentResultBackup> VisualAssessmentResultBackups { get; set; }
        public virtual DbSet<VisualAssessmentTransmission> VisualAssessmentTransmissions { get; set; }
        public virtual DbSet<VisualFieldScore> VisualFieldScores { get; set; }




        #endregion
    }
}
