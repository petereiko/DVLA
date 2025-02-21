using DVLA.Data.Models.BaseFolder;
using System.Collections.Generic;

namespace DVLA.DATA.Domains
{

    public class SmsTemplate : BaseObjectInt32WithoutAuth
    {
        public SmsTemplate()
        {
            SmsTemplateTokens = new HashSet<SmsTemplateToken>();
        }


        public string Name { get; set; }

        public string Body { get; set; }

        public string Subject { get; set; }

        public string Code { get; set; }
        public virtual ICollection<SmsTemplateToken> SmsTemplateTokens { get; set; }
    }
}
