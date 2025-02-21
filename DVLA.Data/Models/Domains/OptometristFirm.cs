using DVLA.Data.Models.Auth;
using DVLA.Data.Models.BaseFolder;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class OptometristFirm : BaseObjectInt32
    {

        [Required]
        [StringLength(150)]
        public string BusinessAddress { get; set; }

        [StringLength(50)]
        public string TelephoneNumber { get; set; }

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

        public int? RegionId { get; set; }
        public int? DistrictId { get; set; }
        public virtual District District { get; set; }
        public bool? IsSynchronized { get; set; }

        [Required]
        [StringLength(150)]
        public string Town { get; set; }

        public int? ReorderLevel { get; set; }

        public virtual Region Region { get; set; }

    }
}
