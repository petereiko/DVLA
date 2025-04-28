using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class PinSetting
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int MaxUseCount { get; set; }
        public decimal Amount { get; set; }
    }
}
