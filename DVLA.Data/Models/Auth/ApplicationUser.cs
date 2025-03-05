using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DVLA.Data.Models.Auth
{
    public class ApplicationUser:IdentityUser<string>
    {
        public string Pin { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public long? DepartmentId { get; set; }
        public int? OptometristFirmId { get; set; }
        public bool IsFirstLogin { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DateUpdated { get; set; }
        public bool IsActive { get; set; }
        public string DefaultRole { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return this.LastName + " " + this.FirstName;
            }
        }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

    }
}
