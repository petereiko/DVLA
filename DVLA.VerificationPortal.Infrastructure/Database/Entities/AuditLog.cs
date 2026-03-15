using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class AuditLog
    {
        public long Id { get; set; }
        public string Action { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
