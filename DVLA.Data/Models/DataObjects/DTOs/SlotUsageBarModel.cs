using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotUsageBarModel
    {
        public int TotalSlot { get; set; }
        public int LearnerUsedSlot { get; set; }
        public int LearnUnusedSlot { get; set;}
        public int OtherUsedSlot { get; set; }
        public int OtherUnusedSlot { get; set; }
    }
}
