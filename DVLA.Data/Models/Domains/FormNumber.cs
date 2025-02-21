using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class FormNumber
    {
        public long Id { get; set; }
        public long LastCount { get; set; }
        public int SerialType { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
