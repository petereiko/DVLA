using DVLA.Data;
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
    public class HardDeleteVisualAssessmentResultJob : IJob
    {
        private readonly ILogger<HardDeleteVisualAssessmentResultJob> _logger;
        private readonly DVLADbContext _context;

        public HardDeleteVisualAssessmentResultJob(ILogger<HardDeleteVisualAssessmentResultJob> logger, DVLADbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                _logger.LogInformation($"Delete Visual Assessment Result Started");

                var visualAssessmentResults = _context.VisualAssessmentResults.Where(x => x.TestDate <= DateTime.UtcNow.AddMonths(-12) && x.IsTransmitted);
                _context.VisualAssessmentResults.RemoveRange(visualAssessmentResults);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
