using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class TestAnalysisViewModel
    {
        public long? OptometristFirmId { get; set; }
        public IEnumerable<OptometristFirmModel> OptometristFirms { get; set; } = Enumerable.Empty<OptometristFirmModel>();
        public string StartDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public string EndDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
        public List<TestAnalysisModel> SlotUsages { get; set; } = new();

    }
}
