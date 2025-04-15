using DVLA.Data.Models.DataObjects.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Data.Models.DataObjects.ViewModels
{
    public class AuditGridViewModel
    {
        public List<ActivityModel> Items { get; set; } = new List<ActivityModel>();
        public List<VisualAssessmentExportDto> ExportItems { get; set; } = new List<VisualAssessmentExportDto>();
        public AuditFilterModel Filter { get; set; }
    }
}
