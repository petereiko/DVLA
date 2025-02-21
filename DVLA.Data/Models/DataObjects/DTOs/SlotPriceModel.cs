using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotPriceModel
    {
        public int Id {  get; set; }
        public bool IsActive { get; set; }
        public decimal Price { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public AccessType AccessType { get; set; }
        public string CreatedByFullName { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByFullName { get;set; }
    }
}
