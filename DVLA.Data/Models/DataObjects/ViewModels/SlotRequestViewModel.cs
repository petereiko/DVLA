using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class SlotRequestViewModel
    {
        public SlotRequestModel FormData { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
