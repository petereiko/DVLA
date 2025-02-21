using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotModel
    {
        public long Id { get; set; }
        public int OptometristFirmId { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public AccessType AccessType { get; set; }
    }
}
