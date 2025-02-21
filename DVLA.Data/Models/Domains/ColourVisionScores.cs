using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    public class ColourVisionScore: BaseObjectInt64
    {

        [StringLength(50)]
        public string Score { get; set; }

    }
}
