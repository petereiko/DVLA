using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotDeductionModel
    {
        public int OptometristFirmId { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
    }
}
