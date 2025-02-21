using DVLA.Data.Models.BaseFolder;
using DVLA.DATA.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Domains
{
    public class ModuleAction:BaseObjectInt64
    {
        public long ModuleId { get; set; }
        public virtual Module Module { get; set; }
        public string ActionName { get; set; }
    }
}
