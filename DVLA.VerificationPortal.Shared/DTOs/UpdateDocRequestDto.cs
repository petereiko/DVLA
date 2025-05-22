using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class UpdateDocRequestDto
    {
        public long Id { get; set; }
        public long VisualAssessmentResultId { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? OptometristName { get; set; }
    }
}
