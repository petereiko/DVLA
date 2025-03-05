using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Domain.Entities
{
    public class EmailAttachment
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public long EmailLogId { get; set; }
        public virtual EmailLog EmailLog { get; set; }
    }
}
