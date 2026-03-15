using System;
using System.Collections.Generic;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLA.VerificationPortal.Infrastructure.Database.Context
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


        public DbSet<ApiClient> ApiClients { get; set; }
        public DbSet<ApiAuditLog> ApiAuditLogs { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }

        public DbSet<OptometristFirm> OptometristFirms { get; set; }
        public DbSet<Pin> Pins { get; set; }
        public DbSet<PinSetting> PinSettings { get; set; }
        public DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
        }
    }
}
