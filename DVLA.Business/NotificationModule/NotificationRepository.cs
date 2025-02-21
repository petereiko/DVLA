using DVLA.DATA.Domains;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models;
using DVLA.Data;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.Business.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DVLA.Business.NotificationModule
{
    public class NotificationRepository : INotificationRepository, IDisposable
    {
        private readonly IRepositoryQuery<EmailLog> _emailQuery;
        private readonly IRepositoryQuery<EmailLogAttachment> _emailLogAttachment;
        private readonly IRepositoryQuery<EmailTemplate> _emailTemplate;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationRepository> _logger;
        private readonly string BaseUrl;

        private readonly DVLADbContext _context;
        public NotificationRepository(IRepositoryQuery<EmailLogAttachment> emailLogAttachment,
            IRepositoryQuery<EmailTemplate> emailTemplate, DVLADbContext context, IRepositoryQuery<EmailLog> emailQuery, IConfiguration configuration, ILogger<NotificationRepository> logger)
        {
            _emailQuery = emailQuery;
            _emailLogAttachment = emailLogAttachment;
            _emailTemplate = emailTemplate;
            _context = context;
            _configuration = configuration;
            _logger = logger;
            BaseUrl = configuration["AppConstants:BaseUrl"];
        }

        public void SendNewAccountCreated(ApplicationUser model, string password, string callbackUrl, DVLADbContext context = null)
        {
            try
            {
                context = context == null ? _context : context;
                string from = _configuration["EmailSettings:FromEmail"];

                string message = $"An account has been created on <a href='{BaseUrl}/Account/Login'>{_configuration["AppConstants:AppNameName"]}</a> with the default password <b>{password}</b>. Kindly login with your email {model.Email} and password {password};  update the password to confirm your account.";


                //format email and attachment to entity

                EmailLog email = new EmailLog
                {
                    Recepient = model.Email,
                    Cc = string.Empty,
                    Bcc = string.Empty,
                    Subject = "New Account Created",
                    Message = message,
                    CreatedDate = DateTime.Now,
                    DateToSend = DateTime.Now,
                    Sender = from,
                    IsSent = false,
                    HasAttachment = false
                };
                context.EmailLogs.Add(email);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public void SendForgotPassword(ApplicationUser model, string callbackUrl)
        {
            List<EmailTokenDto> emailTokens = new List<EmailTokenDto>();

            var template = _emailTemplate.GetAllAsync().GetAwaiter().GetResult().FirstOrDefault(x => x.Code == EmailConstants.ForgotPassword);
            if (template == null)
            {
                throw new Exception("Invalid Email Template");
            }

            foreach (var item in template.EmailTemplateTokens)
            {
                var eToken = new EmailTokenDto();
                if (item.EmailToken.TokenName == "[[NAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.LastName + " " + model.FirstName };
                }
                if (item.EmailToken.TokenName == "[[USERNAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.Email };
                }
                if (item.EmailToken.TokenName == "[[URL]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = callbackUrl };
                }
                emailTokens.Add(eToken);
            }

            SendNotification(template.Id, model.Email, string.Empty, string.Empty, Enumerable.Empty<EmailLogAttachementDto>().ToList(),
               emailTokens);
        }

        public void SendPasswordReset(ApplicationUser model, string callbackUrl)
        {

            List<EmailTokenDto> emailTokens = new List<EmailTokenDto>();

            var template = _emailTemplate.FilterAsync(x => x.Code == EmailConstants.PasswordReset).GetAwaiter().GetResult().FirstOrDefault();
            if (template == null)
            {
                throw new Exception("Invalid Email Template");
            }

            foreach (var item in template.EmailTemplateTokens)
            {
                var eToken = new EmailTokenDto();
                if (item.EmailToken.TokenName == "[[NAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.LastName + " " + model.FirstName };
                }
                if (item.EmailToken.TokenName == "[[USERNAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.Email };
                }
                if (item.EmailToken.TokenName == "[[URL]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = callbackUrl };
                }
                emailTokens.Add(eToken);
            }

            SendNotification(template.Id, model.Email, string.Empty, string.Empty, Enumerable.Empty<EmailLogAttachementDto>().ToList(),
                emailTokens);
        }

        public void SendAssessmentResult(string firstName, string mobileNumber, string referenceNumber, string assessmentResult, string emailTo, DVLADbContext context = null)
        {
            try
            {
                context = context == null ? _context : context;
                StringBuilder emailBody = new StringBuilder();
                emailBody.AppendLine($"<h3>Dear {firstName}</h3>");
                emailBody.AppendLine($"<div>A visual assessment result has been created with the following properties:</div>");
                emailBody.AppendLine($"<div>Reference: {referenceNumber}</div>");
                emailBody.AppendLine($"<div>Date:{DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm tt")}</div>");
                emailBody.AppendLine($"<div>Result: {assessmentResult}</div>");

                string _emailSetting_EmailFrom = _configuration["EmailSettings:SmtpUser"].ToString();

                EmailLog email = new EmailLog
                {
                    Recepient = emailTo,
                    Cc = null,
                    Bcc = null,
                    Subject = "New Visual Assessment Result",
                    Message = emailBody.ToString(),
                    CreatedDate = DateTime.Now,
                    DateToSend = DateTime.Now,
                    Sender = _emailSetting_EmailFrom,
                    IsSent = false,
                    HasAttachment = false,

                };

                context.EmailLogs.Add(email);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task SendRegistrationDetail(string firstName, string mobileNumber, string referenceNumber, string emailTo)
        {
            try
            {
                StringBuilder emailBody = new StringBuilder();

                var emailTemplate = _emailTemplate.FilterAsync(x => x.Code == EmailConstants.RegistrationConfirmation).GetAwaiter().GetResult().FirstOrDefault();
                if (emailTemplate != null)
                {
                    //template.SmsTemplateTokens=_context.Sm

                    emailBody.Append(emailTemplate.EmailBody);

                    emailBody.Replace("[[NAME]]", firstName);
                    emailBody.Replace("[[REFERENCENUMBER]]", referenceNumber);
                    emailBody.Replace("[[DATE]]", DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm tt"));


                    string emailMessage = emailBody.ToString();
                    string _emailSetting_EmailFrom = _configuration["EmailSettings:SmtpUser"].ToString();

                    EmailLog email = new EmailLog
                    {
                        Recepient = emailTo,
                        Cc = null,
                        Bcc = null,
                        Subject = emailTemplate.EmailSubject,
                        Message = emailMessage,
                        CreatedDate = DateTime.Now,
                        DateToSend = DateTime.Now,
                        Sender = _emailSetting_EmailFrom,
                        IsSent = false,
                        HasAttachment = false,

                    };

                    _context.EmailLogs.Add(email);
                    await _context.SaveChangesAsync();


                }
                else
                {
                    _logger.LogInformation("No Template found for Sms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
        private EmailLogDto CreateEmail(long emailID, string emailTo, string cc, string bcc, List<EmailLogAttachementDto> emailAttachCol, List<EmailTokenDto> tokenList)
        {
            var emailTemplate = _emailTemplate.FilterAsync(u => u.Id == emailID).GetAwaiter().GetResult().FirstOrDefault();

            StringBuilder sbEmailBody = new StringBuilder();

            sbEmailBody.Append(emailTemplate.EmailBody);

            foreach (var token in tokenList)
            {
                sbEmailBody = sbEmailBody.Replace(token.Token, token.TokenValue);
            }
            string emailBody = sbEmailBody.ToString();

            EmailLogDto newEmailLog = new EmailLogDto
            {
                Email = emailTo,
                CC = string.IsNullOrEmpty(cc) ? null : cc,
                BCC = string.IsNullOrEmpty(bcc) ? null : bcc,
                Message = emailBody,
                Subject = emailTemplate.EmailSubject,
            };

            if (emailAttachCol != null)
            {
                newEmailLog.attachmentModelCol = emailAttachCol;
            }

            return newEmailLog;
        }


        private void SendNotification(long EmailTemplateId, string emailTo, string emailCC
            , string emailBCC, List<EmailLogAttachementDto> emailAttachCol, List<EmailTokenDto> emailTokenCol, string emailSubject = "")
        {
            var emailModel = CreateEmail(EmailTemplateId, emailTo, emailCC, emailBCC, emailAttachCol, emailTokenCol);

            if (!string.IsNullOrEmpty(emailSubject))
            {
                emailModel.Subject = emailSubject;
            }

            string _emailSetting_EmailFrom = _configuration["EmailSettings:SmtpUser"].ToString();

            //format email and attachment to entity

            EmailLog email = new EmailLog
            {
                Recepient = emailModel.Email,
                Cc = emailModel.CC,
                Bcc = emailModel.BCC,
                Subject = emailModel.Subject,
                Message = emailModel.Message,
                CreatedDate = DateTime.Now,
                DateToSend = DateTime.Now,
                Sender = _emailSetting_EmailFrom,
                IsSent = false,
                HasAttachment = emailModel.hasAttachment,

            };

            _context.EmailLogs.Add(email);
            _context.SaveChanges();
            //_emailLog.Insert(email);
            // _emailLog.SaveChanges();
        }

        public async Task SendReminder(List<RemindersModel> reminders)
        {
            try
            {
                StringBuilder emailBody = new StringBuilder();

                var emailTemplate = _emailTemplate.FilterAsync(x => x.Code == EmailConstants.Reminder).GetAwaiter().GetResult().FirstOrDefault();
                if (emailTemplate != null)
                {
                    //template.SmsTemplateTokens=_context.Sm
                    foreach (var item in reminders)
                    {
                        if (!string.IsNullOrEmpty(item.ContactNumber))
                        {
                            emailBody.Append(emailTemplate.EmailBody);
                            emailBody.Replace("[[NAME]]", item.FirstName);
                            emailBody.Replace("[[REFERENCENUMBER]]", item.ReferenceNumber);
                            emailBody.Replace("[[DUEDATE]]", item.DueDate);
                            emailBody.Replace("[[TESTDATE]]", item.TestDate.ToString("dddd, dd MMMM yyyy"));

                            string emailMessage = emailBody.ToString();
                            string _emailSetting_EmailFrom = _configuration["EmailSettings:SmtpUser"].ToString();

                            EmailLog email = new EmailLog
                            {
                                Recepient = item.Email,
                                Cc = null,
                                Bcc = null,
                                Subject = emailTemplate.EmailSubject,
                                Message = emailMessage,
                                CreatedDate = DateTime.Now,
                                DateToSend = DateTime.Now,
                                Sender = _emailSetting_EmailFrom,
                                IsSent = false,
                                HasAttachment = false,

                            };

                            _context.EmailLogs.Add(email);
                        }

                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    _logger.LogInformation("No Template found for Sms");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
        public void Dispose()
        {
            _context.Dispose();
        }

        public void SlotRequestNotification(ApplicationUser model, int slotQuantity, string requesterId, string callbackUrl)
        {
            List<EmailTokenDto> emailTokens = new List<EmailTokenDto>();

            var template = _emailTemplate.FilterAsync(x => x.Code == EmailConstants.SlotRequest).GetAwaiter().GetResult().FirstOrDefault();
            if (template == null)
            {
                throw new Exception("Invalid Email Template");
            }

            foreach (var item in template.EmailTemplateTokens)
            {
                var eToken = new EmailTokenDto();
                if (item.EmailToken.TokenName == "[[NAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.LastName + " " + model.FirstName };
                }
                if (item.EmailToken.TokenName == "[[USERNAME]]")
                {
                    var requester = _context.ApplicationUsers.FirstOrDefault(x => x.Id == requesterId);
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = requester.LastName + " " + requester.FirstName };
                }
                if (item.EmailToken.TokenName == "[[URL]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = callbackUrl };
                }
                if (item.EmailToken.TokenName == "[[SLOTNUMBER]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = slotQuantity.ToString() };
                }
                emailTokens.Add(eToken);
            }

            SendNotification(template.Id, model.Email, string.Empty, string.Empty, Enumerable.Empty<EmailLogAttachementDto>().ToList(),
                emailTokens);
        }

        public void SlotApprovalNotification(ApplicationUser model, int slotQuantity, string callbackUrl)
        {
            List<EmailTokenDto> emailTokens = new List<EmailTokenDto>();

            var template = _emailTemplate.FilterAsync(x => x.Code == EmailConstants.SlotApproved).GetAwaiter().GetResult().FirstOrDefault();
            if (template == null)
            {
                throw new Exception("Invalid Email Template");
            }

            foreach (var item in template.EmailTemplateTokens)
            {
                var eToken = new EmailTokenDto();
                if (item.EmailToken.TokenName == "[[NAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.LastName + " " + model.FirstName };
                }
                if (item.EmailToken.TokenName == "[[URL]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = callbackUrl };
                }
                if (item.EmailToken.TokenName == "[[SLOTNUMBER]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = slotQuantity.ToString() };
                }
                emailTokens.Add(eToken);
            }

            SendNotification(template.Id, model.Email, string.Empty, string.Empty, Enumerable.Empty<EmailLogAttachementDto>().ToList(),
                emailTokens);
        }

        public void SlotDeclineNotification(ApplicationUser model, int slotQuantity, string callbackUrl)
        {
            List<EmailTokenDto> emailTokens = new List<EmailTokenDto>();

            var template = _emailTemplate.FilterAsync(x => x.Code == EmailConstants.SlotDeclined).GetAwaiter().GetResult().FirstOrDefault();
            if (template == null)
            {
                throw new Exception("Invalid Email Template");
            }

            foreach (var item in template.EmailTemplateTokens)
            {
                var eToken = new EmailTokenDto();
                if (item.EmailToken.TokenName == "[[NAME]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = model.LastName + " " + model.FirstName };
                }
                if (item.EmailToken.TokenName == "[[URL]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = callbackUrl };
                }
                if (item.EmailToken.TokenName == "[[SLOTNUMBER]]")
                {
                    eToken = new EmailTokenDto { Token = item.EmailToken.TokenName, TokenValue = slotQuantity.ToString() };
                }
                emailTokens.Add(eToken);
            }

            SendNotification(template.Id, model.Email, string.Empty, string.Empty, Enumerable.Empty<EmailLogAttachementDto>().ToList(),
                emailTokens);
        }
    }
}
