using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Enums
{
    public enum ResultServiceType
    {
        [Display(Name = "Not Set")]
        NotSet =0,
        [Display(Name = "Learner Driver's Licence")]
        LearnerDriversLicence = 1,
        [Display(Name = "Renewal of Driver’s Licence")]
        RenewalOfDriverLicence = 2,
        [Display(Name = "Replacement of Driver’s Licence")]
        ReplacementOfDriverLicence = 3,
        [Display(Name = "Upgrade of Driver’s Licence")]
        UpgradeOfDriverLicence = 4,
        [Display(Name = "Accident Report")]
        AccidentReport = 5
    }
}
