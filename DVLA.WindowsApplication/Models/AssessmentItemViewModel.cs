using DVLA.WindowsApplication.Data;
using DVLA.WindowsApplication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Models
{
    public class AssessmentItemViewModel
    {
        public long Id { get; set; }

        public int OptometristFirmId { get; set; }

        public string OptometristFirmName { get; set; }

        public string ReferenceNumber { get; set; }

        //public string FormNumber { get; set; }

        public ResultServiceType? ResultServiceType { get; set; }

        public byte TestType { get; set; }

        //public int? NameTitle { get; set; }

        public PassOrFail? PassOrFail { get; set; }

        public string Surname { get; set; }

        //public string DriversLicence { get; set; }

        //public string DVLAReferenceNo { get; set; }

        //public string OldDVLAReferenceNo { get; set; }

        public string FirstName { get; set; }

        //public string OtherName { get; set; }

        //public DateTime? DOB { get; set; }

        //[StringLength(500)]
        //public string PostalAddress { get; set; }

        //[StringLength(50)]
        public string ContactNumber { get; set; }

        //[StringLength(50)]
        public string TIN { get; set; }

        //[StringLength(50)]
        //public string Email { get; set; }



        public string ResultConclusion { get; set; }

        public bool? IsSynchronized { get; set; }

        public DateTime? TestDate { get; set; }

        public string PassportImageUrl { get; set; }

        public Status? Status { get; set; }

        public LearnerDriversLicenceType? LearnerDriversLicence { get; set; }

        //public bool? IsRegistration { get; set; }

        public int? AccessType { get; set; }

        //[Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        //public virtual OptometristFirm OptometristFirm { get; set; }

        public bool IsTransmitted { get; set; }
    }
}
