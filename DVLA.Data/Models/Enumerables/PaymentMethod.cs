using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Enumerables
{
    public enum PaymentMethod
    {
        [Description("Bank Slip Upload")]
        Upload=1,
        [Description("Online")]
        Online
    }
}
