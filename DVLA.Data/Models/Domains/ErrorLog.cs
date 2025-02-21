using DVLA.Data.Models.BaseFolder;

namespace DVLA.DATA.Domains
{

    public partial class ErrorLog : BaseObjectInt64WithoutAuth
    {

        public string Controller { get; set; }

        public string Action { get; set; }

        public string ErrorlineNo { get; set; }

        public string Errormsg { get; set; }

        public string Extype { get; set; }

        public string Exurl { get; set; }

        public string ErrorLocation { get; set; }

     
    }
}
