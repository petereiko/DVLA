using DVLA.Business.EmailModule;
using DVLA.Business.ReportModule;
using DVLA.Data;
using DVLA.DATA.Domains;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.BackgroundJobModule
{
    [DisallowConcurrentExecution]
    public class SendEmailJob : IJob
    {
        private readonly DVLADbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<SendEmailJob> _logger;
        private readonly IReportRepository _reportRepository;

        public SendEmailJob(DVLADbContext context, IEmailService emailService, IReportRepository reportRepository)
        {
            _context = context;
            _emailService = emailService;
            _reportRepository = reportRepository;
        }

        public async Task Execute(IJobExecutionContext context)
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
    }
}
