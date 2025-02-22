using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public enum AccessType
    {

        [Display(Name = "Learner Driver's Licence")]
        LearnerDriversLicence = 1,
        [Display(Name = "Other Licence Categories")]
        OtherLicenceCategory = 2

    }
}
