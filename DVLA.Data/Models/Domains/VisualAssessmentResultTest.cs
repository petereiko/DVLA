using DVLA.Data.Models.BaseFolder;
using System;

namespace DVLA.DATA.Domains
{

    public class VisualAssessmentResultTest:BaseObjectInt64
    {
        public string OptometristFirmCode { get; set; }
        public string OptometristFirmName { get; set; }
        public string OptometristFirmContactPhone { get; set; }
        public string ReferenceNumber { get; set; }
        public string DVLARefNumber { get; set; }
        public string DriversLicenseNo { get; set; }
        public string ServiceTypeId { get; set; }
        public string Title { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public string DOB { get; set; }
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
        public Nullable<System.DateTime> TestDate { get; set; }
        public System.DateTime SynchDate { get; set; }
        public string Optometrist { get; set; }
        public Nullable<int> PassOrFail { get; set; }
        public Nullable<int> PassResultId { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
