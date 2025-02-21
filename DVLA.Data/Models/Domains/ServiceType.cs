using DVLA.Data.Models.BaseFolder;
using System.ComponentModel.DataAnnotations;

namespace DVLA.DATA.Domains
{

    public partial class ServiceType : BaseObjectInt32WithoutAuth
    {

        [StringLength(50)]
        public string Name { get; set; }

      
    }
}
