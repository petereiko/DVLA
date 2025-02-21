using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class Module : BaseObjectInt32WithoutAuth
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
