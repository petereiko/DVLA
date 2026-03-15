using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class OptometristFirm
    {
        public int Id { get; set; }
        public int OptometristFirmId { get; set; }
        public string BusinessAddress { get; set; } = string.Empty;
        public string? TelephoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? CentreCode { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string? AccreditationNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? DigitalAddress { get; set; }
        public string ContactFirstName { get; set; } = string.Empty;
        public string ContactLastName { get; set; } = string.Empty;
        public string ContactPhoneNumber { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public int? RegionId { get; set; }
        public int? DistrictId { get; set; }
        public bool? IsSynchronized { get; set; }
        public string Town { get; set; } = string.Empty;
        public int? ReorderLevel { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
