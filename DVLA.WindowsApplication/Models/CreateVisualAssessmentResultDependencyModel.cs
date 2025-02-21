using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.WindowsApplication.Models
{
    public class CreateVisualAssessmentResultDependencyModel
    {
        public List<IdNameModel<int>> VisualAcuity { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> VisualFieldScores { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> ColourVisionScores { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> SingleImage { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<string>> ResultConclusions { get; set; } = new List<IdNameModel<string>>();
        public List<IdNameModel<int>> ResultServiceTypes { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> LearnerDriversLicenceType { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> PassOrFail { get; set; } = new List<IdNameModel<int>>();


    }
}
