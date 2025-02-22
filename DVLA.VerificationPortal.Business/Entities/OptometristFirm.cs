using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class OptometristFirm
    {
        public int Id { get; set; }

        public int OptometristFirmId { get; set; }

        [Required]
        [StringLength(150)]
        public string? BusinessAddress { get; set; }

        [StringLength(50)]
        public string? TelephoneNumber { get; set; }

        //[Required]
        [StringLength(50)]
        public string? MobileNumber { get; set; }

        public string? CentreCode { get; set; }

        [Required]
        [StringLength(150)]
        public string? BusinessName { get; set; }

        [StringLength(50)]
        public string? AccreditationNumber { get; set; }

        [StringLength(50)]
        public string? RegistrationNumber { get; set; }

        [StringLength(50)]
        public string? DigitalAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string? ContactFirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? ContactLastName { get; set; }

        [Required]
        [StringLength(50)]
        public string? ContactPhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactEmail { get; set; }

        public int? RegionId { get; set; }
        public int? DistrictId { get; set; }
        public bool? IsSynchronized { get; set; }

        [Required]
        [StringLength(150)]
        public string? Town { get; set; }

        public int? ReorderLevel { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

    }
}
