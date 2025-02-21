namespace DVLA.WindowsApplication.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SlotRequest
    {
        public long Id { get; set; }

        public int OptometristFirmId { get; set; }

        public int Quantity { get; set; }

        public int AccessType { get; set; }

        public string PaymentProof { get; set; }

        public int Status { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateApproved { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal AmountPaid { get; set; }

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

        public int PaymentMethod { get; set; }

        public long? InitiatePaystackTransferRequestId { get; set; }

        public virtual OptometristFirm OptometristFirm { get; set; }
    }
}
