using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class VisualAssessmentResultModel
    {
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public long OptometristFirmId { get; set; }
        public string ReferenceNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public PassOrFail? PassOrFail { get; set; }
        public PassResult? PassResult { get; set; }
        public string Surname { get; set; }
        //public string DriversLicence { get; set; }
        //public string DVLAReferenceNo { get; set; }
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
        public bool? IsGHDriveSynchronized { get; set; }
        public DateTime? TestDate { get; set; }
        public DateTime? TestExpiryDate { get; set; }
        public string PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        public string BusinessAddress { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string CentreCode { get; set; }
        public string BusinessName { get; set; }
        public string AccreditationNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public string DigitalAddress { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string UserName { get; set; }
        //public string CreatedByUsername { get; set; }
        //public string CreatedByFullName { get; set; }
        //public string UpdatedByUsername { get; set; }
        public string RegionName { get; set; }
        //public string Optometrist { get; set; }
        public string DistrictName { get; set; }
        public bool? IsRegistration { get; set; }
        //public string FormNumber { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

    }
}
