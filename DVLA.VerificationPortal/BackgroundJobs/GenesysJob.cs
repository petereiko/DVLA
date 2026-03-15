using DVLA.VerificationPortal.Infrastructure.Repositories;
using Quartz;

namespace DVLA.VerificationPortal.BackgroundJobs
{
    public class GenesysJob : IJob
    {
        private readonly ISearchResultService _searchService;
        private readonly ILogger<GenesysJob> _logger;

        public GenesysJob(ISearchResultService searchService, ILogger<GenesysJob> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await _searchService.ProcessGenesysAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }
    }
}
