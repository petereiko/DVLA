using DVLA.Data.Models.BaseFolder;
using DVLA.Data.Models.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class VisualAssessmentResult : BaseObjectInt64
    {
        //FormNumber, NameTitle, DriversLicense, DVLAReferenceNo, OldDVLAReferenceNo


        public int OptometristFirmId { get; set; }
        public virtual OptometristFirm OptometristFirm { get; set; }
       
        [StringLength(50)]
        public string ReferenceNumber { get; set; }
        //[StringLength(50)]
        //public string FormNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public TestType TestType { get; set; }

        //public NameTitle? NameTitle { get; set; }
        public PassOrFail? PassOrFail { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        //[StringLength(50)]
        //public string DriversLicence { get; set; }

        //[StringLength(50)]
        //public string DVLAReferenceNo { get; set; }

        //[StringLength(50)]
        //public string OldDVLAReferenceNo { get; set; }

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

        //[StringLength(500)]
        public string PathologicalRemarks { get; set; }

        //[StringLength(500)]
        public string ResultConclusion { get; set; }

        public bool? IsSynchronized { get; set; }
        //public bool? IsGHDriveSynchronized { get; set; }

        public DateTime? TestDate { get; set; }

        public string PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        //public LearnerDriversLicenceType? LearnerDriversLicence { get; set; }
        public bool? IsRegistration { get; set; }
        public AccessType? AccessType { get; set; }
        public PassResult? PassResult { get; set; }
        public bool IsTransmitted { get; set; }
        public DateTime TransmittedDate { get; set; }
        public Gender? Gender { get; set; }
        public string TransmissionError { get; set; }
        public bool HasTransmissionError { get; set; }
    }
}
