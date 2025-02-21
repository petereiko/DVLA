namespace WinApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ErrorLog
    {
        public long Id { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string ErrorlineNo { get; set; }

        public string Errormsg { get; set; }

        public string Extype { get; set; }

        public string Exurl { get; set; }

        public string ErrorLocation { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        [Column(TypeName = "timestamp")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [MaxLength(8)]
        public byte[] RowVersion { get; set; }
    }
}
