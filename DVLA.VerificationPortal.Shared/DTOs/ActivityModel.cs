using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Shared.DTOs
{
    public class ActivityModel
    {
        public string? Action { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
