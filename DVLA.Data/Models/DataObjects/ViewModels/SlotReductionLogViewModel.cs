using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SlotReductionLogViewModel
    {
        public SlotReductionLogSearchParameter SearchParameter { get; set; }
        public IEnumerable<SlotReductionModel> SlotReductions { get; set; }=Enumerable.Empty<SlotReductionModel>();
        public IEnumerable<OptometristFirmModel> OptometristFirms { get; set; }= Enumerable.Empty<OptometristFirmModel>();
    }
}
