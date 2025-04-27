using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLA.Data
{
    public class VerificationDbContext: IdentityDbContext
    {
        public VerificationDbContext(DbContextOptions<VerificationDbContext> options) : base(options) { }

        public DbSet<VerificationVisualAssessmentResult> VisualAssessmentResults { get; set; }
    }
}
