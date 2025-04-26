using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data
{
    public class AppConstants
    {
        public static string CACHEUSERDATA = "UserData";
        public static string VISUALASSESSMENTSUBMISSION = "VisualAssessmentSubmission";
        public static string TRANSMISSIONDATA = "TransmissionData";
        public static string[] Roles = { AppRoles.SYSTEMADMIN, AppRoles.FACILITYOWNER, AppRoles.OPTOMETRIST, AppRoles.FINANCE, AppRoles.FRONTOFFICER, AppRoles.SLOTMANAGER };
    }
}
