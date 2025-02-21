using DVLA.Data.Models.BaseFolder;

namespace DVLA.DATA.Domains
{
    public partial class EmailTemplateToken : BaseObjectInt32WithoutAuth
    {

        public int EmailTemplateId { get; set; }

        public int EmailTokenId { get; set; }

      

        public virtual EmailTemplate EmailTemplate { get; set; }

        public virtual EmailToken EmailToken { get; set; }
    }
}
