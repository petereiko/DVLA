using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class Message : BaseObjectInt64
    {
        public string Title { get; set; }
        public string Msg { get; set; }
    }
}
