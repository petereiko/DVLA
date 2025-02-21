using DVLA.Business.EmailModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.PaymentModule;
using DVLA.Business.SlotModule;
using DVLA.Data;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public BackgroundJobService(DVLADbContext context, IEmailService emailService, IPaymentService paymentService, ISmsRepository smsRepository)
        {
            _context = context;
            _emailService = emailService;
            _paymentService = paymentService;
            _smsRepository = smsRepository;
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

        public void OpenTickets()
        {

        }
    }
}
