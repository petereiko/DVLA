using DVLA.Data.Models.DataObjects.UtilityObjects;
using System.Collections.Generic;

namespace DVLA.UI.Models
{
    public class CreateVisualAssessmentResultDependencyModel
    {
        public List<IdNameModel<long>> VisualAcuity { get; set; } = new List<IdNameModel<long>>();
        public List<IdNameModel<long>> VisualFieldScores { get; set; } = new List<IdNameModel<long>>();
        public List<IdNameModel<long>> ColourVisionScores { get; set; } = new List<IdNameModel<long>>();
        public List<IdNameModel<long>> SingleImage { get; set; } = new List<IdNameModel<long>>();
        public List<IdNameModel<string>> ResultConclusions { get; set; } = new List<IdNameModel<string>>();
        public List<IdNameModel<int>> ResultServiceTypes { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> LearnerDriversLicenceType { get; set; } = new List<IdNameModel<int>>();
        public List<IdNameModel<int>> PassOrFail { get; set; } = new List<IdNameModel<int>>();
    }
}
