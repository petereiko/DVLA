using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class VerifiedItemGridViewModel
    {
        public DateTime StartDate { get; set; } = Utility.StartOfDay(DateTime.UtcNow.AddDays(-30));
        public DateTime EndDate { get; set; } = Utility.EndOfDay(DateTime.UtcNow);
        public IEnumerable<VerifiedItemDto> Results { get; set; } = Enumerable.Empty<VerifiedItemDto>();
    }
}
