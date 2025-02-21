using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Models
{
    public class UserViewModel
    {
        public string RoleId { get; set; }
        public bool IsDeleted { get; set; }
        public string Id { get; set; }

        public string PIN { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public string MobileNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public RoleViewModel Role { get; set; }
        public string DefaultRole { get; set; }
        public List<RoleViewModel> Roles { get; set; }
        public bool IsFirstLogin { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public int? OptometristFirmId { get; set; }
        public string OptometristFirmName { get; set; }
    }
}
