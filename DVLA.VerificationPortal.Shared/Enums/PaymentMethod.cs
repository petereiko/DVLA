using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public enum PaymentMethod
    {
        [Description("Bank Slip Upload")]
        Upload=1,
        [Description("Online")]
        Online
    }
}
