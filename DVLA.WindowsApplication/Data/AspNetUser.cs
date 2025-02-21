namespace DVLA.WindowsApplication.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AspNetUser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetUser()
        {
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            OptometristFirmUsers = new HashSet<OptometristFirmUser>();
        }

        [StringLength(450)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DOB { get; set; }

        public string MobileNumber { get; set; }

        public string Address { get; set; }

        public long? DepartmentId { get; set; }

        public int? OptometristFirmId { get; set; }

        public bool IsFirstLogin { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateUpdated { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }

        public string CreatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        [StringLength(256)]
        public string UserName { get; set; }

        [StringLength(256)]
        public string NormalizedUserName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        [StringLength(256)]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptometristFirmUser> OptometristFirmUsers { get; set; }
    }
}
