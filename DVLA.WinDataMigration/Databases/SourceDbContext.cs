using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.WinDataMigration.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLA.WinDataMigration.Databases
{
    public class SourceDbContext: IdentityDbContext
    {
        public SourceDbContext(DbContextOptions<SourceDbContext> options) : base(options) { }

        public DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }
    }
}
