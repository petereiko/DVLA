using DVLA.Data.Models.DataObjects.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SlotDeductionViewModel
    {
        public SlotDeductionModel FormData { get; set; }
        //public IEnumerable<OptometristFirmModel> OptometristFirms { get; set; }
        public IEnumerable<SelectListItem> OptometristFirms { get; set; }
        public List<string> Errors = new List<string>();
    }
}
