using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class EmailLog
    {
        public EmailLog()
        {
            EmailAttachments = new HashSet<EmailAttachment>();
        }

        public long Id { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public bool HasAttachment { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentDate { get; set; }
        public virtual ICollection<EmailAttachment> EmailAttachments { get; set; }
    }
}
