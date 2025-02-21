using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ActivityModel
    {
        public long Id { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string FullName { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
