using DVLA.Business.ReportModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [EnableRateLimiting("AuthenticatedRead")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;
        private readonly IVisualAssessmentResultRepository _visualAssessmentResultRepository;

        public ReportsController(IReportRepository reportRepository, IVisualAssessmentResultRepository visualAssessmentResultRepository)
        {
            _reportRepository = reportRepository;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
        }

        [HttpPost("synchronization")]
        public async Task<IActionResult> GetSynchronizationReport([FromBody] SynchronizationReportFilterViewModel model)
        {
            return Ok(await _reportRepository.GetSynchronizationReport(model));
        }

        [HttpPost("customer-synchronization")]
        public async Task<IActionResult> GetCustomerSynchronizationReport([FromBody] SynchronizationReportFilterViewModel model)
        {
            return Ok(await _reportRepository.GetCustomerSynchronizationReport(model));
        }

        [HttpPost("applicant-search")]
        public async Task<IActionResult> ApplicantSearch([FromBody] ClientSearchParameter model)
        {
            return Ok(await _reportRepository.FetchClientSearch(model));
        }

        [HttpPost("applicant-search/old")]
        public IActionResult ApplicantSearchOld([FromBody] ClientSearchParameter model)
        {
            return Ok(_reportRepository.FetchClientSearchOld(model));
        }

        [HttpPost("slot-reduction-logs")]
        public async Task<IActionResult> FetchSlotReductionLogs([FromBody] SlotReductionLogSearchParameter model)
        {
            return Ok(await _reportRepository.FetchSlotReductionLogs(model));
        }

        [HttpPost("visual-assessment-results")]
        public IActionResult FetchVisualAssessmentResults([FromBody] PaginationRequestModel<ClientSearchRequest> model)
        {
            return Ok(_visualAssessmentResultRepository.FetchAssessmentResults(model));
        }

        [HttpGet("visual-assessment-results/{referenceNumber}")]
        public IActionResult FetchVisualAssessmentResult(string referenceNumber)
        {
            var result = _visualAssessmentResultRepository.FetchAssessmentResult(referenceNumber);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("pending-transmissions")]
        public IActionResult FetchPendingTransmissions()
        {
            return Ok(_reportRepository.FetchAllPendingTransmissions());
        }

        [HttpGet("pending-auth-doc-updates")]
        public IActionResult FetchPendingAuthDocUpdates()
        {
            return Ok(_reportRepository.FetchAllPendingAuthDocUpdate());
        }
    }
}
