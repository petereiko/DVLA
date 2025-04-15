using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class VisualAssessmentResultItemViewModel
    {
        public long Id { get; set; }
        public string ResultConclusion { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantAddress { get; set; }
        public string OptometristFirmName { get; set; }
        public string DSReference { get; set; }
        public string TestDate { get; set; }
        public string Grade { get; set; }
        public string Optometrist { get; set; }
        //public PassResult PassResult { get; set; }
    }
}
