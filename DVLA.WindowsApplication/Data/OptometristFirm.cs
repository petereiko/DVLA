namespace DVLA.WindowsApplication.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OptometristFirm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OptometristFirm()
        {
            OptometristFirmUsers = new HashSet<OptometristFirmUser>();
            SlotRequests = new HashSet<SlotRequest>();
            Slots = new HashSet<Slot>();
            VisualAssessmentResults = new HashSet<VisualAssessmentResult>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string BusinessAddress { get; set; }

        [StringLength(50)]
        public string TelephoneNumber { get; set; }

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

        public bool? IsSynchronized { get; set; }

        [Required]
        [StringLength(150)]
        public string Town { get; set; }

        public int? ReorderLevel { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public virtual District District { get; set; }

        public virtual Region Region { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptometristFirmUser> OptometristFirmUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SlotRequest> SlotRequests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Slot> Slots { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VisualAssessmentResult> VisualAssessmentResults { get; set; }
    }
}
