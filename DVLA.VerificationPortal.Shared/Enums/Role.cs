using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public enum Role
    {
        [Description("Super Admin")]
        SuperAdmin=1,

        [Description("Administrator")]
        Administrator=2,

        [Description("Verifier")]
        Verifier=3
    }
}
