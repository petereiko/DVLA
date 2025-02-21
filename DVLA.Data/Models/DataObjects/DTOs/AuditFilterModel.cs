using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class AuditFilterModel
    {
        public string UserId { get; set; }
        public int? OptometristFirmId { get; set; }
        public int? ModuleId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
