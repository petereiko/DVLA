using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class PriceModel
    {
        public AccessType AccessType { get; set; } = 0;
        public decimal SlotMarketPrice { get; set; }
    }
}
