using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public enum TestType : Byte
    {
        [Display(Name = "New Test")]
        NewTest1 = 0,
        [Display(Name = "New")]
        NewTest = 1,
        [Display(Name = "ReTest")]
        ReTest = 2
    }
}
