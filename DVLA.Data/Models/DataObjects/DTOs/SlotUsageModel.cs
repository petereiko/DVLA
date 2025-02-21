using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotUsageModel
    {
        public string BusinessName { get; set; }
        public int TotalSlotPurchased { get; set; }
        public int TotalSlotUsed { get; set; }
        public int Balance { get; set; }
        public string AccessType { get; set; }
    }
}
