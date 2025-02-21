using DVLA.Data.Models.BaseFolder;
using System.Collections.Generic;

namespace DVLA.DATA.Domains
{

    public partial class SmsToken : BaseObjectInt32WithoutAuth
    {
        public SmsToken()
        {
            SmsTemplateTokens = new HashSet<SmsTemplateToken>();
        }

        public string TokenName { get; set; }

        public virtual ICollection<SmsTemplateToken> SmsTemplateTokens { get; set; }
    }
}
