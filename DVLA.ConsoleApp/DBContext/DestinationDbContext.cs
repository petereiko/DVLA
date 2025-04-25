using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DVLA.ConsoleApp.DBContext
{
    public class DestinationDbContext: IdentityDbContext
    {
        public DestinationDbContext(DbContextOptions<DestinationDbContext> options) : base(options) { }

        public DbSet<VisualAssessmentResult> VisualAssessmentResults { get; set; } 
    }
}
