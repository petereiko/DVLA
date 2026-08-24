using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class VisualAssessmentTransmissionModel
    {
        public long Id { get; set; }

        public int OptometristFirmId { get; set; }

        public string ReferenceNumber { get; set; }

        public string FormNumber { get; set; }

        public int? ResultServiceType { get; set; }

        public byte TestType { get; set; }

        public int? NameTitle { get; set; }

        public PassOrFail? PassOrFail { get; set; }

        public string Surname { get; set; }

        public string DriversLicence { get; set; }

        public string DVLAReferenceNo { get; set; }

        public string OldDVLAReferenceNo { get; set; }

        public string FirstName { get; set; }

        public string OtherName { get; set; }

        public DateTime? DOB { get; set; }

        public string PostalAddress { get; set; }

        public string ContactNumber { get; set; }

        public string TaxIdentificationNumber { get; set; }

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
        public string ContrastSensitivity_BCV { get; set; }

        public string PathologicalRemarks { get; set; }

        public string ResultConclusion { get; set; }

        public bool? IsSynchronized { get; set; }

        public DateTime? TestDate { get; set; }

        public string PassportImageUrl { get; set; }
        public string PassportBase64 { get; set; }

        public Status? Status { get; set; }

        public int? LearnerDriversLicence { get; set; }

        public bool? IsRegistration { get; set; }

        public int? AccessType { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public int? PassResult { get; set; }

        public bool IsTransmitted { get; set; }

        //public string NationalID { get; set; }
        //public string PassportNumber { get; set; }
        public DateTime? TestExpiryDate { get; set; }
    }
}
