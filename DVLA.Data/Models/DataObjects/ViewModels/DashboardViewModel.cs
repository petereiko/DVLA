using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class DashboardViewModel
    {
        public int LearnerGrantedSlotCount { get; set; }
        public int LearnerUtilizedSlotCount { get; set; }
        public int OtherGrantedSlotCount { get; set; }
        public int OtherUtilizedSlotCount { get;set; }
    }
}
