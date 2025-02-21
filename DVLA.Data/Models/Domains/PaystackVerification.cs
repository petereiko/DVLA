using DVLA.Data.Models.BaseFolder;
using DVLA.DATA.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Domains
{
    public class PaystackVerification : BaseObjectInt64
    {
        public long SlotRequestId { get; set; }
        public long? TranId {  get; set; }
        public virtual SlotRequest SlotRequest { get; set; }
        public bool Success { get; set; }
        public int RetryCount { get; set; }
        public string Reference { get; set; }
        public string VerificationData { get; set; }
    }
}
