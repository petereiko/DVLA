using DVLA.Data.Models.BaseFolder;
using System.ComponentModel.DataAnnotations;

namespace DVLA.Data.Models.Domains
{
    

    public partial class ActivityLog : BaseObjectInt64
    {
        [Required]
        public string NameOfUser { get; set; }

        [Required]
        public long ModuleActionId { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
