using DVLA.Data.Models.BaseFolder;

namespace DVLA.DATA.Domains
{
    public partial class SmsTemplateToken : BaseObjectInt32WithoutAuth
    {

        public int SmsTemplateId { get; set; }

        public int SmsTokenId { get; set; }

        public virtual SmsTemplate SmsTemplate { get; set; }

        public virtual SmsToken SmsToken { get; set; }
    }
}
