using DVLA.VerificationPortal.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class VisualAssessmentResult
    {
        public long Id { get; set; }
        public long VisualAssessmentResultId { get; set; }
        public int OptometristFirmId { get; set; }

        public string? OptometristFirmName { get; set; }
        public string? OptometristName { get; set; }
        public Gender? Gender { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public TestType TestType { get; set; }

        public PassOrFail? PassOrFail { get; set; }

        [StringLength(50)]
        public string? Surname { get; set; }

        //[StringLength(50)]
        //public string? OldDVLAReferenceNo { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? OtherName { get; set; }

        public DateTime? DOB { get; set; }

        [StringLength(500)]
        public string? PostalAddress { get; set; }

        [StringLength(50)]
        public string? ContactNumber { get; set; }

        [StringLength(50)]
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

        [StringLength(500)]
        public string? ContrastSensitivity_BCV { get; set; }

        //[StringLength(500)]
        public string? PathologicalRemarks { get; set; }

        //[StringLength(500)]
        public string? ResultConclusion { get; set; }

        //public bool? IsSynchronized { get; set; }
        //public bool? IsGHDriveSynchronized { get; set; }

        public DateTime? TestDate { get; set; }

        public string? PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        public LearnerDriversLicenceType? LearnerDriversLicence { get; set; }
        public bool? IsRegistration { get; set; }
        public AccessType? AccessType { get; set; }
        public PassResult? PassResult { get; set; }
        //public bool IsTransmitted { get; set; }
        public DateTime? TransmittedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        //public DateTime? ModifiedDate { get; set; }
        public string? CreatedBy { get; set; }

        public bool IsVerified { get; set; }
        public DateTime? VerifiedDate { get; set; }
        //public string ModifiedBy { get; set; }
        //public bool IsActive { get; set; } = true;
        //public bool IsDeleted { get; set; } = false;
    }
}
