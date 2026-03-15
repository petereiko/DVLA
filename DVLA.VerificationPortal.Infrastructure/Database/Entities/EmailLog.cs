using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class EmailLog
    {
        public long Id { get; set; }
        public string Email { get; set; } = default!;
        public string? Message { get; set; }
        public DateTime CreatedDate { get; set;}
        public bool HasAttachment { get; set; }
        public bool IsSent { get; set; }
        public string? Subject { get; set; }
    }
}
