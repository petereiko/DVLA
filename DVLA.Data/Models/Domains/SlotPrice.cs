using DVLA.Data.Models.BaseFolder;
using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class SlotPrice: BaseObjectInt32
    {
        public decimal Price { get; set; }
        public AccessType AccessType { get; set; }
    }
}
