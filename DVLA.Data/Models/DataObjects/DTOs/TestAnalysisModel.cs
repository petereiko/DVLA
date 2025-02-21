using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class TestAnalysisModel
    {
        public string BusinessName { get; set; }
        public DateTime? TestDate { get; set; }
        public int Quantity { get; set; }
        public string Metric { get; set; }
        public string Region { get; set; }
    }
}
