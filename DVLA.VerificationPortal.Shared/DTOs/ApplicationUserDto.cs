using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? CentreName { get; set; }
        public string?  UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsFirstLogin { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? Role { get; set; }
    }
}
