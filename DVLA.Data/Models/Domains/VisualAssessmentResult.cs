using DVLA.Data.Models.BaseFolder;
using DVLA.Data.Models.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class VisualAssessmentResult : BaseObjectInt64
    {
        public int OptometristFirmId { get; set; }
        public virtual OptometristFirm OptometristFirm { get; set; }
       
        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        public ResultServiceType? ResultServiceType { get; set; }

        public TestType TestType { get; set; }

        public PassOrFail? PassOrFail { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string OtherName { get; set; }

        public DateTime? DOB { get; set; }

        [StringLength(500)]
        public string PostalAddress { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string Nationality { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        public string Unaided_OD { get; set; }

        public string Unaided_OS { get; set; }

        public string Unaided_OU { get; set; }

        public string BCV_OD { get; set; }

        public string BCV_OS { get; set; }

        public string BCV_OU { get; set; }

        public string HX_BCV_OD { get; set; }

        public string HX_BCV_OS { get; set; }

        public string HX_BCV_OU { get; set; }

        public string SingleImage_BCV_OU { get; set; }

        public string GlareTest_BCV_OD { get; set; }

        public string GlareTest_BCV_OS { get; set; }

        public string GlareTest_BCV_OU { get; set; }

        public string ColourVision_BCV_OU { get; set; }

        [StringLength(500)]
        public string ContrastSensitivity_BCV { get; set; }

        public string PathologicalRemarks { get; set; }

        public string ResultConclusion { get; set; }

        public bool? IsSynchronized { get; set; }

        public DateTime? TestDate { get; set; }

        public string PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        public bool? IsRegistration { get; set; }
        public AccessType? AccessType { get; set; }
        public PassResult? PassResult { get; set; }
        public bool IsTransmitted { get; set; }
        public DateTime? TransmittedDate { get; set; }
        public Gender? Gender { get; set; }
        public string TransmissionError { get; set; }
        public bool HasTransmissionError { get; set; }
        public bool? OptometristNameIsUpdate { get; set; }
        public bool? IsBackedUp { get; set; } = false;
        public DateTime? BackupDate { get; set; } = DateTime.UtcNow;

        public string NationalID { get; set; }
        public string PassportNumber { get; set; }
        public string DvlaLicenseNumber { get; set; }

    }
}
