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
    public class DestinationDbContext: IdentityDbContext
    {
        public DestinationDbContext(DbContextOptions<DestinationDbContext> options) : base(options) { }

        public DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; }
    }
}
