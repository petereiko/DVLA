using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Requests
{
    public class OnboardUserRequest
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
