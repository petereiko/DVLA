using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class IncomeReportViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
        public List<SlotRequestModel> SlotRequests = new();
    }
}
