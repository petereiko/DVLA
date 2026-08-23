using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ApplicantModel:BaseViewModel
    {
        public ApplicantModel()
        {
            IdentityTypes = Enum.GetValues(typeof(IdentityType))
            .Cast<IdentityType>()
            .Select(e => new SelectListItem
            {
                Value = ((int)e).ToString(),
                Text = EnumHelper.GetDescription(e)
            }).ToList();
        }
        public long Id { get; set; }
        
        public PassResult? PassResult{ get; set; }

        [Required(ErrorMessage ="Gender is required")]
        public Gender? Gender { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        [DisplayName("Optometrist Firm")]
        public int OptometristFirmId { get; set; }
        public string ReferenceNumber { get; set; }
        //public string FormNumber { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        //public NameTitle? NameTitle { get; set; }
        public TestType TestType { get; set; }
        public string Surname { get; set; }
        public string Fullname { get; set; }
        // public string DriversLicence { get; set; }
        //public string DVLAReferenceNo { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public DateTime? DOB { get; set; }
        public string DateOfBirth { get; set; }
        public string PostalAddress { get; set; }
        public string ContactNumber { get; set; }
        public string Nationality { get; set; }
        public string Email { get; set; }
        //public string InvoiceNumber { get; set; }
        public string PassportImageUrl { get; set; }
        public Status? Status { get; set; }

        public string Optometrist { get; set; }
        public IFormFile Image { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public bool? IsRegistration { get; set; }
        public string Filename { get; set; }

        public string DisplayImageUrl { get; set; }
        public bool VideoCapture { get; set; }

        [Required(ErrorMessage = "Please select an Identity Type.")]
        public IdentityType IdentityType { get; set; }

        //[Required(ErrorMessage = "Please enter identity number.")]
        //public string IdentityNumber { get; set; }

        public List<SelectListItem> IdentityTypes { get; set; }

        public string DvlaLicenseNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string PassportNumber { get; set; }
        public string GhanaCardNumber { get; set; }

    }
}
