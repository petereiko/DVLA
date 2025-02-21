using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class BaseViewModel
    {
        public List<string> Errors { get; set; } = new();
    }
}
