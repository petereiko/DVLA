using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class UpdateDocRequestDto
    {
        public long VisualAssessmentResultId { get; set; }
        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string OptometristName { get; set; }
    }
}
