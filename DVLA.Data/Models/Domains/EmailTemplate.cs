using DVLA.Data.Models.BaseFolder;
using System.Collections.Generic;

namespace DVLA.DATA.Domains
{
    public partial class EmailTemplate : BaseObjectInt32WithoutAuth
    {
        public EmailTemplate()
        {
            EmailTemplateTokens = new HashSet<EmailTemplateToken>();
        }

        public string EmailName { get; set; }

        public string EmailBody { get; set; }

        public string EmailSubject { get; set; }

        public string Code { get; set; }

        public virtual ICollection<EmailTemplateToken> EmailTemplateTokens { get; set; }
    }
}
