using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public enum PassResult
    {
        [Description("Limited for 3 Months")]
        ThreeMonths=1,

        [Description("Limited for 6 Months")]
        SixMonths = 2,

        [Description("Unlimited")]
        Unlimited = 3,

    }
}
