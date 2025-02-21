using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ApplicantModel:BaseViewModel
    {
        public long Id { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        [DisplayName("Optometrist Firm")]
        public int OptometristFirmId { get; set; }
        public string ReferenceNumber { get; set; }
        public string FormNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public LearnerDriversLicenceType? LearnerDriversLicence { get; set; }
        public NameTitle? NameTitle { get; set; }
        public TestType TestType { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        public string DriversLicence { get; set; }
        public string DVLAReferenceNo { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public DateTime? DOB { get; set; }
        public string DateOfBirth { get; set; }
        public string PostalAddress { get; set; }
        public string ContactNumber { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string Email { get; set; }
        public string InvoiceNumber { get; set; }
        public string PassportImageUrl { get; set; }
        public IFormFile PassportData { get; set; }
        public Status? Status { get; set; }
        public string CreatedByUsername { get; set; }
        public string CreatedByFullName { get; set; }
        public string UpdatedByUsername { get; set; }

        public string Optometrist { get; set; }
        public IFormFile Image { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRegistration { get; set; }
        public string Filename { get; set; }



    }
}
