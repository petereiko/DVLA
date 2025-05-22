using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class VerifiedItemDto
    {
        public int Count { get; set; }
        public string? VerifierEmail { get; set; }
        public string? UserId { get; set; }
    }
}
