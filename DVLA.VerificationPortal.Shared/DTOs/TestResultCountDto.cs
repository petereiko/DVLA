using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Shared.Enums;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class TestResultCountDto
    {
        public int Count { get; set; }
        public ResultServiceType ResultServiceType { get; set; }
        public PassOrFail PassOrFail { get; set; }
        public string? OptometristFirmName { get; set; }
    }
}
