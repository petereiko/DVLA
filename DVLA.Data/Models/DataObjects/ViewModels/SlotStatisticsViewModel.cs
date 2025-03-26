using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SlotStatisticsViewModel:BaseViewModel
    {
        public SlotStatisticsFilterViewModel SlotStatisticsFilter { get; set; }
        public int? OptometristFirmId { get; set; }
        public List<SlotStatisticsItemViewModel> Items { get; set; } = new();
        public int TotalQuantity { get; set; }
    }
}
