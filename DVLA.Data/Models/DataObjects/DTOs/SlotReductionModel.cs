using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class SlotReductionModel
    {
        public long Id { get; set; }
        public long OptometristFirmId { get; set; }
        public string OptometristFirm { get; set; }
        public string Comment { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByFullName { get; set; }
    }
}
