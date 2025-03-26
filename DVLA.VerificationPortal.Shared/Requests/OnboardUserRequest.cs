using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Requests
{
    public class OnboardUserRequest
    {
        [Required]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }

        [Required]
        public string? CentreName { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<SelectListItem> Roles { get; set; } = new();
    }
}
