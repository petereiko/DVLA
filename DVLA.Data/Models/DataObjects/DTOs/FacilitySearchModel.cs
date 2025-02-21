using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class FacilitySearchModel
    {
        public string SearchParameter { get; set; }
        public IEnumerable<OptometristFirmModel> facilities { get; set; } = Enumerable.Empty<OptometristFirmModel>();
    }
}
