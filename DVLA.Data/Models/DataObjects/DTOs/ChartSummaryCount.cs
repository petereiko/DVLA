using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ChartSummaryCount
    {
        public int LearnerValue { get; set; }
        public int OthersValue { get; set; }
        public int MonthlyLearnerValue { get; set; }
        public int MonthlyOthersValue { get; set; }
    }
}
