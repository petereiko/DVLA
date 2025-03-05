using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<EmailLog> _emailLogRepository;
        private readonly IGenericRepository<EmailAttachment> _emailAttachmentRepository;

        public NotificationService(IGenericRepository<EmailLog> emailLogRepository, IGenericRepository<EmailAttachment> emailAttachmentRepository)
        {
            _emailLogRepository = emailLogRepository;
            _emailAttachmentRepository = emailAttachmentRepository;
        }

        public async Task LogEmail(string email, string message, string subject)
        {
            EmailLog emailLog = new() { Email = email, Message = message, Subject = subject };
            await _emailLogRepository.AddAsync(emailLog);
        }

        public async Task LogEmailWithAttachmentAsync(string email, string message, string subject, List<string> attachmentFileNames)
        {
            EmailLog emailLog = new() { Email = email, Message = message, Subject = subject };
            emailLog = await _emailLogRepository.AddAsync(emailLog);

            foreach (var attachmentFile in attachmentFileNames)
            {
                EmailAttachment emailAttachment = new() { EmailLogId = emailLog.Id, FileName = attachmentFile };
                await _emailAttachmentRepository.AddAsync(emailAttachment);
            }   
        }
    }
}
