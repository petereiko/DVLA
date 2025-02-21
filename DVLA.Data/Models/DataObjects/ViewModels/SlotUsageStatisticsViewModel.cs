using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SlotUsageStatisticsViewModel
    {
        public IEnumerable<ChartModel> PerDay { get; set; }
        public IEnumerable<ChartModel> PerWeek { get; set; }
        public IEnumerable<ChartModel> PerMonth { get; set; }
        public IEnumerable<ChartModel> PerYear { get; set; }
    }
}
