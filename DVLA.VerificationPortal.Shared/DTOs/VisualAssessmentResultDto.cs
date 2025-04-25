using DVLA.VerificationPortal.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class VisualAssessmentResultDto
    {
        public long Id { get; set; }
        //public string? EncodedKey { get; set; }
        public long VisualAssessmentResultId { get; set; }
        public int OptometristFirmId { get; set; }
        public Gender? Gender { get; set; }
        public string OptometristFirmName { get; set; }
        public string OptometristName { get; set; }
        public string Nationality { get; set; }
        public string? ReferenceNumber { get; set; }

        //public string? FormNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public TestType TestType { get; set; }

        public PassOrFail? PassOrFail { get; set; }


        public string? Surname { get; set; }

        public string? FirstName { get; set; }

        public string? OtherName { get; set; }

        public DateTime? DOB { get; set; }

        public string? PostalAddress { get; set; }

        public string? ContactNumber { get; set; }

        public string? Email { get; set; }

        public string? Unaided_OD { get; set; }

        public string? Unaided_OS { get; set; }

        public string? Unaided_OU { get; set; }

        public string? BCV_OD { get; set; }

        public string? BCV_OS { get; set; }

        public string? BCV_OU { get; set; }

        public string? HX_BCV_OD { get; set; }

        public string? HX_BCV_OS { get; set; }

        public string? HX_BCV_OU { get; set; }

        public string? SingleImage_BCV_OU { get; set; }

        public string? GlareTest_BCV_OD { get; set; }

        public string? GlareTest_BCV_OS { get; set; }

        public string? GlareTest_BCV_OU { get; set; }

        public string? ColourVision_BCV_OU { get; set; }

        public string? ContrastSensitivity_BCV { get; set; }

        public string? PathologicalRemarks { get; set; }
        public string? ResultConclusion { get; set; }

        public DateTime? TestDate { get; set; }

        public string? PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        public bool? IsRegistration { get; set; }
        public AccessType? AccessType { get; set; }
        public PassResult? PassResult { get; set; }
        public DateTime TransmittedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
    }
}
