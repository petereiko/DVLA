using DVLA.Data.Models.BaseFolder;

namespace DVLA.DATA.Domains
{
    public partial class EmailLogAttachment : BaseObjectInt32WithoutAuth
    {

        public string FolderOnServer { get; set; }

        public string FileNameOnServer { get; set; }

        public string EmailFileName { get; set; }

        public long EmailLogId { get; set; }

     

        public virtual EmailLog EmailLog { get; set; }
    }
}
