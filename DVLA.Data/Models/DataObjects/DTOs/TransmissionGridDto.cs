using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.DATA.Domains;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class TransmissionGridDto
    {
        public TransmissionGridDto()
        {
            RequestDto = new()
            {
                SqlQuery = "select * from VisualAssessmentResults where CreatedDate between '2025-03-11 10:16:03.8952335' and '2025-03-17 20:03:03.8952335'",
                SourceConnectionString = "Server=195.250.23.229;Database=DVLAVerificationDB;User Id=admin_verify;password=267tp8Va@;Encrypt=false;TrustServerCertificate=true;MultipleActiveResultSets=true;"

            };
        }
        public IEnumerable<VerificationVisualAssessmentResult> Results { get; set; } = Enumerable.Empty<VerificationVisualAssessmentResult>();
        public TransmissionRequestDto RequestDto { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
    }
}
