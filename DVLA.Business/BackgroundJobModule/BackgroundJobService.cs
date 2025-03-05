using DVLA.Business.EmailModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.PaymentModule;
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
        public BackgroundJobService(DVLADbContext context, IEmailService emailService, IPaymentService paymentService, ISmsRepository smsRepository, IConfiguration configuration, IHostingEnvironment hostEnvironment, ILogger<BackgroundJobService> logger)
        {
            _context = context;
            _emailService = emailService;
            _paymentService = paymentService;
            _smsRepository = smsRepository;
            _configuration = configuration;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }

        [DisableConcurrentExecution(60)]
        public void SendBulkEmail()
        {

            List<EmailLog> emailLogs = _context.EmailLogs.Where(x => !x.IsSent && x.RetryCount <= 5 && !string.IsNullOrEmpty(x.Recepient)).Take(10).ToList();
            foreach (var item in emailLogs)
            {
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


        [DisableConcurrentExecution(60)]
        public void SendBulkSms()
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

        [DisableConcurrentExecution(60)]
        public void VerifyPayments()
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

        [DisableConcurrentExecution(60)]
        public void PushVisualAssessmentResult()
        {
            try
            {
                var visualAssessmentResults = _context.VisualAssessmentResults.AsNoTracking().Where(x => x.CreatedDate < DateTime.Now.AddMonths(-3)).Take(20)
                .Select(x => new VisualAssessmentResultDto
                {
                    AccessType = x.AccessType,
                    BCV_OD = x.BCV_OD,
                    BCV_OS = x.BCV_OS,
                    BCV_OU = x.BCV_OU,
                    ColourVision_BCV_OU = x.ColourVision_BCV_OU,
                    ContactNumber = x.ContactNumber,
                    ContrastSensitivity_BCV = x.ContrastSensitivity_BCV,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    DOB = x.DOB,
                    DriversLicence = x.DriversLicence,
                    DVLAReferenceNo = x.DVLAReferenceNo,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    FormNumber = x.FormNumber,
                    GlareTest_BCV_OD = x.GlareTest_BCV_OD,
                    GlareTest_BCV_OS = x.GlareTest_BCV_OS,
                    GlareTest_BCV_OU = x.GlareTest_BCV_OU,
                    HX_BCV_OD = x.HX_BCV_OD,
                    HX_BCV_OS = x.HX_BCV_OD,
                    HX_BCV_OU = x.HX_BCV_OD,
                    IsRegistration = x.IsRegistration,
                    NameTitle = x.NameTitle,
                    LearnerDriversLicence = x.LearnerDriversLicence,
                    OptometristFirmId = x.OptometristFirmId,
                    OtherName = x.OtherName,
                    PassOrFail = x.PassOrFail,
                    PassResult = x.PassResult,
                    PassportImageUrl = x.PassportImageUrl,
                    PathologicalRemarks = x.PathologicalRemarks,
                    PostalAddress = x.PostalAddress,
                    ReferenceNumber = x.ReferenceNumber,
                    ResultConclusion = x.ResultConclusion,
                    ResultServiceType = x.ResultServiceType,
                    SingleImage_BCV_OU = x.SingleImage_BCV_OU,
                    Status = x.Status,
                    Surname = x.Surname,
                    TaxIdentificationNumber = x.TaxIdentificationNumber,
                    TestDate = x.TestDate,
                    TestType = x.TestType,
                    TransmittedDate = DateTime.UtcNow,
                    Unaided_OD = x.Unaided_OD,
                    Unaided_OS = x.Unaided_OS,
                    Unaided_OU = x.Unaided_OU,
                    VisualAssessmentResultId = x.Id,
                    Id = x.Id
                });

                foreach (var item in visualAssessmentResults)
                {
                    using var content = new MultipartFormDataContent();

                    var json = JsonConvert.SerializeObject(item);
                    content.Add(new StringContent(json, Encoding.UTF8, "application/json"), "VisualAssessmentResult");

                    if (!string.IsNullOrEmpty(item.PassportImageUrl))
                    {
                        var filePath = Path.Combine(_hostEnvironment.WebRootPath, "Passports", item.PassportImageUrl);
                        if (File.Exists(filePath))
                        {
                            var fileStream = File.OpenRead(filePath);
                            var streamContent = new StreamContent(fileStream);
                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            content.Add(streamContent, "Passport", item.PassportImageUrl);
                        }
                    }
                    using var client = new HttpClient();
                    var response = client.PostAsync(_configuration["AppConstants:ApiVerificationPushUrl"], content).GetAwaiter().GetResult();
                    if (response.IsSuccessStatusCode)
                    {
                        MessageResponse result = JsonConvert.DeserializeObject<MessageResponse>(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
                        if (result.Success)
                        {
                            var visualAssessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.Id);
                            if (visualAssessmentResult != null)
                            {
                                _context.VisualAssessmentResults.Remove(visualAssessmentResult);
                                _context.SaveChanges();
                            }
                        }
                        else if (result.Message == "Record Exists")
                        {
                            var visualAssessmentResult = _context.VisualAssessmentResults.FirstOrDefault(x => x.Id == item.Id);
                            if (visualAssessmentResult != null)
                            {
                                _context.VisualAssessmentResults.Remove(visualAssessmentResult);
                                _context.SaveChanges();
                            }
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
