using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class AdminSlotRequestViewModel:BaseViewModel
    {
        public SlotRequestParameter parameter { get; set; }
        public IEnumerable<SlotRequestModel> Slots { get; set; } = Enumerable.Empty<SlotRequestModel>();
    }
}
