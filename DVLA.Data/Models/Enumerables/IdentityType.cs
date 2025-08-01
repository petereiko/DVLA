using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Enumerables
{
    public enum IdentityType
    {
        [Description("National ID Card")]
        NationalIDCard = 0,

        [Description("International Passport")]
        InternationalPassport = 1
    }
}
