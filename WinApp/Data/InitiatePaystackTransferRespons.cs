namespace WinApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InitiatePaystackTransferResponses")]
    public partial class InitiatePaystackTransferRespons
    {
        public long Id { get; set; }

        public bool Status { get; set; }

        public string Message { get; set; }

        public string AuthorizationUrl { get; set; }

        public string AccessCode { get; set; }

        public string Reference { get; set; }

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

        public long? InitiatePaystackTransferRequestId { get; set; }

        public virtual InitiatePaystackTransferRequest InitiatePaystackTransferRequest { get; set; }
    }
}
