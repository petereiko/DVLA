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
        [Description("Ghanaian")]
        NationalIDCard = 0,

        [Description("International")]
        InternationalPassport = 1
    }
}
