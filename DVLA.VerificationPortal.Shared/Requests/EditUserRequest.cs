using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Requests
{
    public class EditUserRequest
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool EmailConfirmed { get; set; }
        public List<SelectListItem> Roles { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
