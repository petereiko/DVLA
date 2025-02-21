using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApp.Data;

namespace WinApp.Models
{
    public class VisualAssessmentTransmissionModel
    {
        public string PassportBase64 { get; set; }

        public long Id { get; set; }

        public int OptometristFirmId { get; set; }

        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        [StringLength(50)]
        public string FormNumber { get; set; }

        public int? ResultServiceType { get; set; }

        public byte TestType { get; set; }

        public int? NameTitle { get; set; }

        public int? PassOrFail { get; set; }

        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(50)]
        public string DriversLicence { get; set; }

        [StringLength(50)]
        public string DVLAReferenceNo { get; set; }

        [StringLength(50)]
        public string OldDVLAReferenceNo { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string OtherName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DOB { get; set; }

        [StringLength(500)]
        public string PostalAddress { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string TaxIdentificationNumber { get; set; }

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

        [Column(TypeName = "datetime2")]
        public DateTime? TestDate { get; set; }

        public string PassportImageUrl { get; set; }

        public int? Status { get; set; }

        public int? LearnerDriversLicence { get; set; }

        public bool? IsRegistration { get; set; }

        public int? AccessType { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public int? PassResult { get; set; }

        public virtual OptometristFirm OptometristFirm { get; set; }
        public bool IsTransmitted { get; set; }
        public DateTime? TransmittedDate { get; set; }
    }
}
