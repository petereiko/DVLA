using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLA.VerificationPortal.Infrastructure.Database.Context
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }

        public DbSet<OptometristFirm> OptometristFirms { get; set; }
        public DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
        }
    }
}
