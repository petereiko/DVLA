namespace DVLA.WindowsApplication.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class AspNetUserRole
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(450)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(450)]
        public string RoleId { get; set; }

        [Required]
        [StringLength(34)]
        public string Discriminator { get; set; }

        public virtual AspNetRole AspNetRole { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
