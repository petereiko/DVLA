using DVLA.VerificationPortal.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class VisualAssessmentResult
    {
        public long Id { get; set; }
        public long VisualAssessmentResultId { get; set; }
        public int OptometristFirmId { get; set; }
        public string? ReferenceNumber { get; set; }
        public int? ResultServiceType { get; set; }
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
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedDate { get; set; }
        public string? VerifiedBy { get; set; }
        public VerifyType? VerifyType { get; set; }
        public Gender? Gender { get; set; }
        public string? OptometristFirmName { get; set; }
        public string? OptometristName { get; set; }
        public string? Nationality { get; set; }
        public string? NationalID { get; set; }
        public string? PassportNumber { get; set; }
        public string? DvlaLicenseNumber { get; set; }
        public bool? GenesisIsTranmitted { get; set; }
        public DateTime? GenesisTransmittedDate { get; set; }
        public string? GenesisResponseCode { get; set; }
        public string? GenesisStatus { get; set; }
        public string? GenesisError { get; set; }
        public string? GenesisMessage { get; set; }
        public string? InvoiceNumber { get; set; }

    }
}
