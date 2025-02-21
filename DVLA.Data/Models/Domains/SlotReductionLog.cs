using DVLA.Data.Models.BaseFolder;
using DVLA.Data.Models.Enumerables;

namespace DVLA.DATA.Domains
{
    public class SlotReductionLog: BaseObjectInt64
    {
        public int OptometristFirmId { get; set; }
        //public virtual OptometristFirm OptometristFirm { get; set; }
        public int Quantity { get; set; }
        public AccessType AccessType { get; set; }
        public string Comment { get; set; }
    }
}
