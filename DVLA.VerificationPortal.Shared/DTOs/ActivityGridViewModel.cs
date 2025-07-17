using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class ActivityGridViewModel
    {
        public AuditFilterModel? Filter { get; set; }
        public List<ActivityModel> Activities { get; set; } = new();
    }
}
