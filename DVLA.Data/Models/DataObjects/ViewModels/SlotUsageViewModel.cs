using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SlotUsageViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        public AccessType? AccessType { get; set; }
        public IEnumerable<SlotUsageModel> SlotUsages { get; set; }=Enumerable.Empty<SlotUsageModel>();
        public decimal TotalBalance { get; set; }
        public int TotalAccessPurchase { get; set; }
        public int TotalAccessUsed { get; set; }

    }
}
