using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.DTOs
{
    public class VisualAssessmentResultListItem
    {
        public long Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string DVLAReferenceNo { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string OtherName { get; set; }
        public Status Status { get; set; }
        public string PassResult { get; set; }
        public ResultServiceType? ResultServiceType { get; set; }
        public DateTime? TestDate { get; set; }
        public string FullName
        {
            get
            {
                return (FirstName + " " + Surname + " " + OtherName).Trim();
            }
        }
    }
}
