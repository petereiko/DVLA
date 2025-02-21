using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SynchronizationReportFilterViewModel
    {
        public Int64? RegionId { get; set; }
        public bool IsAdministrator { get; set; }
        public Int64? OptometristFirmId { get; set; }
        public string CenterCode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public PassOrFail Result { get; set; }
        public List<CustomerReportViewModel> Reports { get; set; } = new();
    }
}
