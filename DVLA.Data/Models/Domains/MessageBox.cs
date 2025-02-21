using DVLA.Data.Models.BaseFolder;

namespace DVLA.DATA.Domains
{

    public partial class MessageBox:BaseObjectInt64
    {
        public long? MessageID { get; set; }

        public long? RecipientID { get; set; }

        public bool? IsRead { get; set; }

    }
}
