using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class TestResultDto
    {
        public string? FullName { get; set; }
        public string? PassConclusion { get; set; }
        public bool Verified { get; set; }
    }
}
