using DVLA.Data.Models.BaseFolder;
using DVLA.Data.Models.Domains;
using DVLA.Data.Models.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class SlotRequest : BaseObjectInt64
    {
        public int OptometristFirmId { get; set; }
        public virtual OptometristFirm OptometristFirm { get; set; }
        public int Quantity { get; set; }
        public AccessType AccessType { get; set; }

        public string PaymentProof { get; set; }

        public SlotRequestStatus Status { get; set; }


        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime? DateApproved { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public long? InitiatePaystackTransferRequestId { get; set; }

    }
}
