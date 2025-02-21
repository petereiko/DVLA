using DVLA.Data.Models.Auth;
using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.DATA.Domains
{
    [Table("OptometristFirmUsers")]
    public partial class OptometristFirmUser : BaseObjectInt64
    {
        public int OptometristFirmId { get; set; }
        public virtual OptometristFirm OptometristFirm { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
