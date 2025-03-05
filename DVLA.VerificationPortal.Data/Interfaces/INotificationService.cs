using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface INotificationService
    {
        Task LogEmail(string email, string message, string subject);
        Task LogEmailWithAttachmentAsync(string email, string message, string subject, List<string> attachmentFileNames);
    }
}
