using DVLA.DATA.Domains;
using DVLA.Data.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class UserModel
    {
        public string Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Optometrist Firm")]
        public int? OptometristFirmId { get; set; }
        public string OptometristFirmName { get; set; }
        public string RoleId { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<ApplicationRole> ApplicationRoles { get; set; }
        public List<OptometristFirm> OptometristFirms { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
