using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Database.Entities
{
    public class EmailAttachment
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public int EmailLogId { get; set; }
    }
}
