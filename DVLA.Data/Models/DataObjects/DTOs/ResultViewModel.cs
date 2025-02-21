using DVLA.Data.Models.DataObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class ResultViewModel
    {
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<VisualAssessmentResultItemViewModel> aaData { get; set; } = new();

    }
}
