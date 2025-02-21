using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DVLA.Data.Models.Domains
{
    public class VisualAssessmentTransmission
    {
        public long Id { get; set; }
        public bool IsTransmitted { get; set; }
        public DateTime TransmittedDate { get; set; }
        public int RecordCount { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Data { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
