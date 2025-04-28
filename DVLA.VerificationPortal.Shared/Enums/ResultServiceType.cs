using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.Enums
{
    public enum ResultServiceType
    {
        [Description("Learner Driver's Licence")]
        [Display(Name = "Learner Driver's Licence")]
        LearnerDriversLicence = 1,

        [Description("Renewal of Driver’s Licence")]
        [Display(Name = "Renewal of Driver’s Licence")]
        RenewalOfDriverLicence = 2,

        [Description("Replacement of Driver’s Licence")]
        [Display(Name = "Replacement of Driver’s Licence")]
        ReplacementOfDriverLicence = 3,

        [Description("Upgrade of Driver’s Licence")]
        [Display(Name = "Upgrade of Driver’s Licence")]
        UpgradeOfDriverLicence = 4,

        [Description("Accident Report")]
        [Display(Name = "Accident Report")]
        AccidentReport = 5
    }
}
