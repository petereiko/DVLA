using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class TestResultDto
    {
        public string? FullName { get; set; }
        public string? PassConclusion { get; set; }
        public bool Verified { get; set; }
        public string? Passport { get; set; }
        public string? TestType { get; set; }
        public DateTime? TestDate { get; set; }
        public string? IdentityType { get; set; }
        public string? IdentityNumber { get; set; }
        public string? DvlaLicenseNumber { get; set; }
    }
}
