using DVLA.DATA.Domains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;

namespace DVLA.Business.EmailModule
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly DVLADbContext _context;

        public EmailService(IConfiguration configuration, DVLADbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool> LogEmail(EmailLogDto model)
        {
            bool isLogged = false;
            try
            {
                var log = new EmailLog
                {
                    Recepient = model.Email,
                    Subject = model.Subject,
                    Message = model.Message,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    IsSent = false,
                    RetryCount = 0,
                    CreatedBy = "System"
                };
                _context.EmailLogs.Add(log);
                isLogged = await _context.SaveChangesAsync() > 0;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return isLogged;
        }


        public bool SendEmail(string email, string subject, string message)
        {
            bool isSent = false;
            if (string.IsNullOrEmpty(email)) return false;

            var smtpSettings = _configuration.GetSection("EmailSettings");

            var smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
            {
                Port = int.Parse(smtpSettings["SmtpPort"]),
                Credentials = new NetworkCredential(smtpSettings["SmtpUser"], smtpSettings["SmtpPass"]),
                EnableSsl = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SmtpUser"], smtpSettings["SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            try
            {
                smtpClient.Send(mailMessage);
                isSent = true;
            }
            catch (SmtpException smtpEx)
            {
                // Log SMTP-specific exceptions
                _logger.LogError(smtpEx.Message, smtpEx);
                throw new Exception("There was an issue sending the email. Please try again later.");
            }
            catch (Exception ex)
            {
                // Log general exceptions
                _logger.LogError(ex.Message, ex);
                throw new Exception("An unexpected error occurred while sending the email. Please try again later.");
            }
            return isSent;
        }


        public void SendEmailToAllSubscribers(string subject, string message, List<string> emails)
        {
            foreach (var email in emails)
            {
                SendEmail(email, subject, message);
            }
        }

        public bool SendEmailWithAttachment(string email, string subject, string message, Attachment attachment)
        {
            bool isSent = false;
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Invalid email address");
            }

            var smtpSettings = _configuration.GetSection("EmailSettings");

            var smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
            {
                Port = int.Parse(smtpSettings["SmtpPort"]),
                Credentials = new NetworkCredential(smtpSettings["SmtpUser"], smtpSettings["SmtpPass"]),
                EnableSsl = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);
            mailMessage.Attachments.Add(attachment);

            try
            {
                smtpClient.Send(mailMessage);
                isSent = true;
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx.Message, smtpEx);
                throw new Exception("There was an issue sending the email. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw new Exception("An unexpected error occurred while sending the email. Please try again later.");
            }
            return isSent;
        }
    }
}
