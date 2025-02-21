using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class SlotReOrderLevel:BaseObjectInt64
    {
        public int QuantityLevel { get; set; }
    }
}
