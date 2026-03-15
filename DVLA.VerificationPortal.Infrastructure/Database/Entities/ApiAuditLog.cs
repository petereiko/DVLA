using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class ApiAuditLog
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public int ApiClientId { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
