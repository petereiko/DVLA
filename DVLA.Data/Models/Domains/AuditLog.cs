using DVLA.Data.Models.Auth;
using DVLA.Data.Models.BaseFolder;

namespace DVLA.DATA.Domains
{


    public class AuditLog : BaseObjectInt64
    {

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
