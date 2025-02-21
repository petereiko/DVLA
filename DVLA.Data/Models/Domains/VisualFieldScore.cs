using DVLA.Data.Models.BaseFolder;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class VisualFieldScore : BaseObjectInt64
    {

        [StringLength(50)]
        public string Score { get; set; }

      
    }
}
