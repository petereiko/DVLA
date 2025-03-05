using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotRequestModel
    {
        public long Id { get; set; }
        public int OptometristFirmId { get; set; }
        public string BusinessName { get; set; }
        public string TelephoneNumber { get; set; }

        [Required(ErrorMessage = "Please retype Amount Paid to automatically compute the corresponding number of slots")]
        public int Quantity { get; set; }
        [Required]
        public AccessType AccessType { get; set; } = 0;
        public string PaymentProof { get; set; }
        //[Required(ErrorMessage = "Upload Payment Evidence in .jpg or .pdf format")]
        public IFormFile PostedFile { get; set; }

        public SlotRequestStatus Status { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateApproved { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal? AmountPaid { get; set; }
        public List<PriceModel> SlotPriceList { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Online;


    }
}
