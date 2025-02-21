using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class SerialNumber:BaseObjectInt64
    {
        public long LastCount { get; set; }
        public int SerialType { get; set; }
    }
}
