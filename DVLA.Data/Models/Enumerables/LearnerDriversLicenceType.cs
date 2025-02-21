using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.Enumerables
{
    public enum LearnerDriversLicenceType
    {
        [Display(Name = "New Learner Driver’s Licence")]
        NewLearnerDriverLicence = 1,
        [Display(Name = "Renewal of Learner Driver’s Licence")]
        RenewalOfLearnerDriverLicence = 2,
    }
}
