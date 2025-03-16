
using DVLA.Data.Models.BaseFolder;
using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class Applicant : BaseObjectInt64
    {
        public int OptometristFirmId { get; set; }

        [Required]
        [StringLength(50)]
        public string ReferenceNumber { get; set; }

        public ResultServiceType? ResultServiceType { get; set; }
        public TestType? TestType { get; set; }

        //public NameTitle? NameTitle { get; set; }  

        [StringLength(50)]
        public string Surname { get; set; }

        //[StringLength(50)]
        //public string DriversLicence { get; set; }

        //[StringLength(50)]
        //public string DVLAReferenceNo { get; set; }

        //[StringLength(50)]
        //public string InvoiceNumber { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string OtherName { get; set; }
        //  [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]

        public DateTime? DOB { get; set; }

        [StringLength(500)]
        public string PostalAddress { get; set; }

        [StringLength(50)]
        public string ContactNumber { get; set; }

        [StringLength(50)]
        public string TaxIdentificationNumber { get; set; }

        [StringLength(50)]
        public string Email { get; set; }
        public string PassportImageUrl { get; set; }
        public Status? Status { get; set; }
        public LearnerDriversLicenceType? LearnerDriversLicence { get; set; }
        //public string OldDVLAReferenceNo { get; set; }
        //public string FormNumber { get; set; }
        public bool IsRegistration { get; set; }
    }
}
