using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Shared.Enums;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class TestResultCountGridViewModel
    {
        public DateTime StartDate { get; set; } = Utility.StartOfDay(DateTime.UtcNow.AddDays(-30));
        public DateTime EndDate { get; set; } = Utility.EndOfDay(DateTime.UtcNow);
        public IEnumerable<TestResultCountDto> Results { get; set; } = Enumerable.Empty<TestResultCountDto>();
        public PassOrFail? PassOrFail { get; set; }
    }
}
