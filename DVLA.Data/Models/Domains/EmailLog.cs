using DVLA.Data.Models.BaseFolder;
using System;
using System.Collections.Generic;

namespace DVLA.DATA.Domains
{


    public class EmailLog : BaseObjectInt64
    {
        public EmailLog()
        {
            EmailLogAttachments = new HashSet<EmailLogAttachment>();
        }

        public int RetryCount { get; set; }
        public string Sender { get; set; }

        public string Recepient { get; set; }

        public string Cc { get; set; }

        public string Bcc { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool IsSent { get; set; }

        public DateTime? DateSent { get; set; }

        public DateTime DateToSend { get; set; }

        public bool HasAttachment { get; set; }

    
        public virtual ICollection<EmailLogAttachment> EmailLogAttachments { get; set; }
    }
}
