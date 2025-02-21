using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinApp.Models
{
    public class ApplicationUser
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string MobileNumber { get; set; }

        public string Address { get; set; }

        public long? DepartmentId { get; set; }

        public int? OptometristFirmId { get; set; }

        public bool IsFirstLogin { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? DateUpdated { get; set; }

        public byte[] RowVersion { get; set; }

        public string CreatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }

        public string SecurityStamp { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public string DefaultRole { get; set; }

        public string ModifiedBy { get; set; }

        public string Pin { get; set; }
    }
}
