using DVLA.Data;
using DVLA.Data.Models;
using DVLA.DATA.Domains;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DVLA.Business.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.Extensions.Options;

namespace DVLA.Business.NotificationModule
{
    public class SmsRepository : ISmsRepository, IDisposable
    {
        private readonly DVLADbContext _context;
        private readonly IRepositoryQuery<SmsTemplate> _templateQuery;
        private readonly SmsSettings _smsSettings;
        private readonly ILogger<SmsRepository> _logger;
        public SmsRepository(DVLADbContext context, IRepositoryQuery<SmsTemplate> templateQuery, IOptions<SmsSettings> options, ILogger<SmsRepository> logger)
        {
            _context = context;
            _templateQuery = templateQuery;
            _smsSettings = options.Value;
            _logger = logger;
        }


        public async Task<Tuple<bool, string>> SendSmsIntegration(string message, string mobileNumber)
        {
            var result = new Tuple<bool, string>(false, string.Empty);
            if (!mobileNumber.StartsWith("233")) mobileNumber = "233" + mobileNumber;
            try
            {
                string encodedAuthKey = WebUtility.UrlEncode(_smsSettings.smsAuthKey);
                message = WebUtility.UrlEncode(message);
                string endPoint = _smsSettings.smsEndpoint;
                string uri = $"{endPoint}/?key={encodedAuthKey}&type=0&destination={mobileNumber}&dlr=1&source=NALO&message={message}";

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    string[] tokens = jsonResponse.Split("|");
                    result = new Tuple<bool, string>(tokens[0].Trim() == "1701", tokens[2]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return result;
        }

        public async Task SendPendingSms()
        {
            try
            {
                string motif1 = @"^([0-9]{10})$"; //@"^\(?([0-9]{3})\)?[-. ]?([0-9]{4})[-. ]?([0-9]{4})$";
                                                  //string motif2 = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                string motif3 = @"^\+?\d*$";

                List<ItemToSendViewModel> emailLog = new List<ItemToSendViewModel>();
                var smsRequests = _context.SmsLogs.Where(x => !x.IsSent).Take(50).ToList();
                if (smsRequests.Count > 0)
                {
                    foreach (var sms in smsRequests)
                    {
                        string responseId = "";
                        if (!Regex.IsMatch(sms.MobileNumber, motif1) && !Regex.IsMatch(sms.MobileNumber, motif3))
                        {
                            //var smsDetails = _context.SmsLogs.FirstOrDefault(x => x.Id == sms.Id);
                            //smsDetails.IsSent = true;
                            //smsDetails.ResponseId = responseId;
                            //smsDetails.UpdatedOn = DateTime.Now;
                            //await _context.SaveChangesAsync();
                            emailLog.Add(new ItemToSendViewModel { Id = sms.Id, IsSent = true, responseId = "bad phone number" });
                        }
                        //bool sendSms = SendSmsIntegration(sms.Message, sms.MobileNumber, out responseId);
                        var sendSms = await SendSmsIntegration(sms.Message, sms.MobileNumber);
                        if (sendSms.Item1)
                        {
                            //var smsDetails = _context.SmsLogs.FirstOrDefault(x => x.Id == sms.Id);
                            //smsDetails.IsSent = true;
                            //smsDetails.ResponseId = responseId;
                            //smsDetails.UpdatedOn = DateTime.Now;

                            //await _context.SaveChangesAsync();
                            emailLog.Add(new ItemToSendViewModel { Id = sms.Id, IsSent = true, responseId = sendSms.Item2 });
                        }

                    }

                    if (emailLog.Count > 0)
                    {

                        try
                        {
                            //context.Configuration.AutoDetectChangesEnabled = false;

                            // Make many calls in a loop

                            foreach (var item in emailLog)
                            {
                                SmsLog toupdate = _context.SmsLogs.Where(p => p.Id == item.Id).FirstOrDefault();
                                toupdate.IsSent = item.IsSent;
                                toupdate.ModifiedDate = DateTime.Now;
                                toupdate.ResponseId = item.responseId;
                            }

                            _context.SaveChanges();
                        }
                        finally
                        {
                            //context.Configuration.AutoDetectChangesEnabled = true;
                        }

                    }
                }
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        //public void HandleVisualAssessmentNotification()
        //{
        //    VisualAssessmentController
        //}

        public void SendAssessmentResult(string firstName, string mobileNumber, string referenceNumber, string assessmentResult, DVLADbContext context = null)
        {
            try
            {
                context = context == null ? _context : context;
                StringBuilder sbSmsBody = new StringBuilder();
                sbSmsBody.AppendLine($"Dear {firstName}\r\n");
                sbSmsBody.AppendLine($"Reference Number: {referenceNumber}\r\n");
                sbSmsBody.AppendLine($"Date: {DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm tt")}\r\n");
                sbSmsBody.AppendLine($"Result: {assessmentResult}\r\n");

                var sms = new SmsLog
                {
                    Message = sbSmsBody.ToString(),
                    MobileNumber = mobileNumber,
                    IsSent = false
                };
                context.SmsLogs.Add(sms);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task SendRegistrationDetail(string firstName, string mobileNumber, string referenceNumber)
        {
            try
            {
                StringBuilder sbSmsBody = new StringBuilder();

                var template = _templateQuery.FilterAsync(x => x.Code == SmsTokenConstants.Registration).Result.FirstOrDefault();
                if (template != null)
                {
                    //template.SmsTemplateTokens=_context.Sm

                    sbSmsBody.Append(template.Body);

                    sbSmsBody.Replace("[[NAME]]", firstName);
                    sbSmsBody.Replace("[[REFERENCENUMBER]]", referenceNumber);
                    sbSmsBody.Replace("[[DATE]]", DateTime.UtcNow.ToString("dddd, dd MMMM yyyy hh:mm tt"));




                    var sms = new SmsLog
                    {
                        Message = sbSmsBody.ToString(),
                        MobileNumber = mobileNumber
                    };
                    _context.SmsLogs.Add(sms);
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

        public async Task SendReminder(List<RemindersModel> reminders)
        {
            try
            {
                StringBuilder sbSmsBody = new StringBuilder();

                var smsTemplate = _templateQuery.FilterAsync(x => x.Code == SmsTokenConstants.Reminder).Result.FirstOrDefault();
                var emailTemplate = _templateQuery.FilterAsync(x => x.Code == SmsTokenConstants.Reminder).Result.FirstOrDefault();
                if (smsTemplate != null)
                {
                    //template.SmsTemplateTokens=_context.Sm
                    foreach (var item in reminders)
                    {
                        if (!string.IsNullOrEmpty(item.ContactNumber))
                        {
                            sbSmsBody.Append(smsTemplate.Body);
                            sbSmsBody.Replace("[[NAME]]", item.FirstName);
                            sbSmsBody.Replace("[[REFERENCENUMBER]]", item.ReferenceNumber);
                            sbSmsBody.Replace("[[DUEDATE]]", item.DueDate);
                            sbSmsBody.Replace("[[TESTDATE]]", item.TestDate.ToString("dddd, dd MMMM"));

                            var sms = new SmsLog
                            {
                                Message = sbSmsBody.ToString(),
                                MobileNumber = item.ContactNumber,
                                IsSent = false
                            };
                            _context.SmsLogs.Add(sms);
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
    }
}
