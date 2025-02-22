using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class ApplicationUser:IdentityUser<string>
    {
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
