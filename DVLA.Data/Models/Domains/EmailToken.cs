using DVLA.Data.Models.BaseFolder;
using System.Collections.Generic;

namespace DVLA.DATA.Domains
{
    public partial class EmailToken : BaseObjectInt32WithoutAuth
    {
        public EmailToken()
        {
            EmailTemplateTokens = new HashSet<EmailTemplateToken>();
        }


        public string TokenName { get; set; }

        public string Token { get; set; }

        public virtual ICollection<EmailTemplateToken> EmailTemplateTokens { get; set; }
    }
}
