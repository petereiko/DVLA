using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class OptometristFirmModel:BaseViewModel
    {
        public int Id { get; set; }
        public int SlotBalance { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CentreCode { get; set; }
        public int? ReorderLevel { get; set; }
        [Required]
        [Display(Name = "Business Address")]
        public string BusinessAddress { get; set; }
        [Required]
        [Display(Name = "Business Telephone Number")]
        public string TelephoneNumber { get; set; }
        //[Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Required]
        [Display(Name = "Business Name")]
        public string BusinessName { get; set; }
        //[Required]
        [Display(Name = "Accreditation Number")]
        public string AccreditationNumber { get; set; }
        [Required]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }
        [Required]
        [Display(Name = "Digital Address")]
        public string DigitalAddress { get; set; }
        [Required]
        [Display(Name = "Contact Person's Other Name")]
        public string ContactFirstName { get; set; }
        [Required]
        [Display(Name = "Contact Person's Last Name")]
        public string ContactLastName { get; set; }
        [Required]
        [Display(Name = "Contact Person's Phone Number")]
        public string ContactPhoneNumber { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Contact Person's Email Address")]
        public string ContactEmailAddress { get; set; }
        [Required]
        [Display(Name = "Region")]
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        [Required]
        [Display(Name = "Town")]
        public string Town { get; set; }
        public IEnumerable<SelectListItem> Regions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Districts { get; set; } = Enumerable.Empty<SelectListItem>();
        [Required]
        [Display(Name = "District")]
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public IList<SelectListItem> DistrictNames { get; set; }
    }
}
