using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Models
{
    public class OptometristFirmViewModel : BaseViewModel
    {

        //public List<SelectListItem> Regions { get; set; } = new();
        //public List<SelectListItem> Districts { get; set; } = new();
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        public string BusinessAddress { get; set; }

        [StringLength(50)]
        public string TelephoneNumber { get; set; }

        public string DistrictName { get; set; }

        //[Required]
        [StringLength(50)]
        public string MobileNumber { get; set; }

        public string CentreCode { get; set; }

        [Required]
        [StringLength(150)]
        public string BusinessName { get; set; }

        [StringLength(50)]
        public string AccreditationNumber { get; set; }

        [StringLength(50)]
        public string RegistrationNumber { get; set; }

        [StringLength(50)]
        public string DigitalAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactFirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactLastName { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactPhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string ContactEmail { get; set; }

        [Required]
        public int? RegionId { get; set; }

        [Required]
        public int? DistrictId { get; set; }
        public bool? IsSynchronized { get; set; }

        [Required]
        [StringLength(150)]
        public string Town { get; set; }

        public int? ReorderLevel { get; set; }

        public string RegionName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
