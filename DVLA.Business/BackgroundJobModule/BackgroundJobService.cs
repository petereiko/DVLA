using DVLA.Business.EmailModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.PaymentModule;
using DVLA.Business.ReportModule;
using DVLA.Business.SlotModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.BackgroundJobModule
{
    public class BackgroundJobService
    {
        private readonly DVLADbContext _context;
        private readonly IEmailService _emailService;
        private readonly ISmsRepository _smsRepository;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostEnvironment;
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IReportRepository _reportRepository;

        public BackgroundJobService(DVLADbContext context, IEmailService emailService, IPaymentService paymentService, ISmsRepository smsRepository, IConfiguration configuration, IHostingEnvironment hostEnvironment, ILogger<BackgroundJobService> logger, IReportRepository reportRepository)
        {
            _context = context;
            _emailService = emailService;
            _paymentService = paymentService;
            _smsRepository = smsRepository;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _reportRepository = reportRepository;
        }

        [DisableConcurrentExecution(60)]
        public void SendBulkEmail()
        {
            try
            {
                List<EmailLog> emailLogs = _context.EmailLogs.Where(x => !x.IsSent && x.RetryCount <= 5).Take(10).ToList();
                foreach (var item in emailLogs)
                {
                    if (string.IsNullOrEmpty(item.Recepient))
                    {
                        item.RetryCount = 6;
                        _context.SaveChanges();
                        continue;
                    }
                    bool isValid = _emailService.IsValidEmail(item.Recepient);
                    if (!isValid)
                    {
                        item.RetryCount = 6;
                        _context.SaveChanges();
                        continue;
                    }

                    bool result = _emailService.SendEmail(item.Recepient, item.Subject, item.Message);
                    if (result)
                    {
                        item.IsSent = true;
                        item.ModifiedDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                    else
                    {
                        item.RetryCount++;
                        item.ModifiedDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
            }


        }


        [DisableConcurrentExecution(60)]
        public void SendBulkSms()
        {
            try
            {
                List<SmsLog> smsLogs = _context.SmsLogs.Where(x => !x.IsSent && x.RetryCount <= 5).Take(10).ToList();
                foreach (var item in smsLogs)
                {
                    var result = _smsRepository.SendSmsIntegration(item.MobileNumber, item.Message).GetAwaiter().GetResult();
                    if (result.Item1)
                    {
                        item.IsSent = true;
                        item.ModifiedDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                    else
                    {
                        item.RetryCount++;
                        item.ModifiedDate = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
            }



        }

        [DisableConcurrentExecution(60)]
        public void VerifyPayments()
        {
            try
            {
                var slotRequests = _context.SlotRequests.Where(x => x.Status == SlotRequestStatus.Pending && x.PaymentMethod == PaymentMethod.Online).Take(10);
                int count = slotRequests.Count();
                if (count > 0)
                {
                    foreach (var slotRequest in slotRequests)
                    {
                        _paymentService.VerifyPayment(slotRequest.ReferenceNumber);
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex.Message, ex);
            }

        }

        [DisableConcurrentExecution(60)]
        public void PushVisualAssessmentResult()
        {
            try
            {
                _logger.LogInformation($"Push Visuai Assessment Result Started");

                bool runPushAssessment = Convert.ToBoolean(_configuration["AppConstants:RunPushAssessmentResult"]);

                _logger.LogInformation($"Service Started: {runPushAssessment}");

                if (!runPushAssessment) { return; }

                var visualAssessmentResults = _reportRepository.FetchAllPendingTransmissions();

                _logger.LogInformation($"{visualAssessmentResults.Count} results found");

                foreach (VisualAssessmentResultDto item in visualAssessmentResults)
                {
                    try
                    {
                        using var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AppConstants:ApiVerificationPushUrl"]);
                        request.Headers.Add("X-API-KEY", _configuration["AppConstants:ApiKey"]);
                        var requestBody = JsonConvert.SerializeObject(item);
                        _logger.LogInformation($"Request Body {requestBody}");
                        var content = new StringContent(requestBody, null, "application/json");
                        request.Content = content;
                        var response = client.SendAsync(request).GetAwaiter().GetResult();
                        _logger.LogInformation($"Response Object: {JsonConvert.SerializeObject(response)}");
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonSuccess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            MessageResponse messageResponse = JsonConvert.DeserializeObject<MessageResponse>(jsonSuccess);
                            if (messageResponse.Success)
                            {
                                var visualAssessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.VisualAssessmentResultId);

                                visualAssessmentResult.IsTransmitted = true;
                                visualAssessmentResult.TransmittedDate = messageResponse.Message.Equals("Record Exists") ? visualAssessmentResult.TransmittedDate : DateTime.UtcNow;
                                visualAssessmentResult.TransmissionError = null;
                                visualAssessmentResult.HasTransmissionError = false;
                                _context.SaveChanges();
                            }
                        }
                        else
                        {

                            string errorContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                            _logger.LogInformation($"Error Object: {errorContent}");

                            var assessment = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.Id);
                            assessment.IsTransmitted = false;
                            assessment.HasTransmissionError = true;
                            assessment.TransmissionError = errorContent;
                            _context.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Could not reach the Push API");
                        _logger.LogError(ex.Message, ex);
                        break;
                    }

                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }


        [DisableConcurrentExecution(60)]
        public void UpdateAuthDoc()
        {
            try
            {
                _logger.LogInformation($"Update Started");

                var visualAssessmentResults = _reportRepository.FetchAllPendingAuthDocUpdate();

                _logger.LogInformation($"{visualAssessmentResults.Count} results found");

                foreach (UpdateDocRequestDto item in visualAssessmentResults)
                {
                    try
                    {
                        using var client = new HttpClient();
                        var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AppConstants:ApiVerificationUpdateDocUrl"]);
                        request.Headers.Add("X-API-KEY", _configuration["AppConstants:ApiKey"]);
                        var requestBody = JsonConvert.SerializeObject(item);
                        _logger.LogInformation($"Request Body {requestBody}");
                        var content = new StringContent(requestBody, null, "application/json");
                        request.Content = content;
                        var response = client.SendAsync(request).GetAwaiter().GetResult();
                        _logger.LogInformation($"Response Object: {JsonConvert.SerializeObject(response)}");
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonSuccess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            MessageResponse messageResponse = JsonConvert.DeserializeObject<MessageResponse>(jsonSuccess);
                            if (messageResponse.Success)
                            {
                                VisualAssessmentResult visualAssessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.VisualAssessmentResultId);

                                visualAssessmentResult.OptometristNameIsUpdate = true;
                                _context.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogInformation("Could not reach the Push API");
                        _logger.LogError(ex.Message, ex);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }


        [DisableConcurrentExecution(60)]
        public void HardDeleteVisualAssessmentResults()
        {
            try
            {
                _logger.LogInformation($"Delete Visual Assessment Result Started");

                var visualAssessmentResults = _context.VisualAssessmentResults.Where(x => x.TestDate <= DateTime.UtcNow.AddMonths(-3) && x.IsTransmitted); //_reportRepository.FetchAllPendingTransmissions();
                _context.VisualAssessmentResults.RemoveRange(visualAssessmentResults);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }



        [DisableConcurrentExecution(60)]
        public void SyncOptometristFirms()
        {
            try
            {
                _logger.LogInformation($"Update Started");

                var optometristFirms = _context.OptometristFirms.AsNoTracking().Where(x => x.IsSynchronized != true).ToList();

                if (optometristFirms.Count == 0) return;

                _logger.LogInformation($"{optometristFirms.Count} results found");

                IEnumerable<OptometristFirmTransmissionDto> transmissions = optometristFirms
                    .Select(x => new OptometristFirmTransmissionDto
                    {
                        AccreditationNumber = x.AccreditationNumber,
                        BusinessAddress = x.BusinessAddress,
                        BusinessName = x.BusinessName,
                        CentreCode = x.CentreCode,
                        ContactEmail = x.ContactEmail,
                        ContactFirstName = x.ContactFirstName,
                        ContactLastName = x.ContactLastName,
                        ContactPhoneNumber = x.ContactPhoneNumber,
                        CreatedBy = x.CreatedBy,
                        CreatedDate = x.CreatedDate,
                        DigitalAddress = x.DigitalAddress,
                        DistrictId = x.DistrictId,
                        IsActive = x.IsActive,
                        IsDeleted = x.IsDeleted,
                        IsSynchronized = true,
                        MobileNumber = x.MobileNumber,
                        ModifiedBy = string.IsNullOrEmpty(x.ModifiedBy)?"":x.ModifiedBy,
                        ModifiedDate = x.ModifiedDate,
                        OptometristFirmId = x.Id,
                        RegionId = x.RegionId,
                        RegistrationNumber = x.RegistrationNumber,
                        ReorderLevel = x.ReorderLevel,
                        TelephoneNumber = x.TelephoneNumber,
                        Town = x.Town
                    });

                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["AppConstants:ApiVerificationTransmitOpometristFirmsUrl"]);
                request.Headers.Add("X-API-KEY", _configuration["AppConstants:ApiKey"]);
                var requestBody = JsonConvert.SerializeObject(transmissions);
                _logger.LogInformation($"Request Body {requestBody}");
                var content = new StringContent(requestBody, null, "application/json");
                request.Content = content;
                var response = client.SendAsync(request).GetAwaiter().GetResult();
                _logger.LogInformation($"Response Object: {JsonConvert.SerializeObject(response)}");
                var jsonSuccess = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    
                    List<int> returnedOptometristIds = JsonConvert.DeserializeObject<List<int>>(jsonSuccess);
                    if (returnedOptometristIds.Count > 0)
                    {

                        foreach (var id in returnedOptometristIds)
                        {
                            var opt = _context.OptometristFirms.FirstOrDefault(x => x.Id == id);
                            opt.IsSynchronized = true;
                            _context.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

        }
    }
}
