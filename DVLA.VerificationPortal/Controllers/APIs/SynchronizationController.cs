using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.CustomAttributes;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class SynchronizationController : ControllerBase
    {
        private readonly ISearchResultService _searchService;
        private readonly ILogger<SynchronizationController> _logger;

        public SynchronizationController(ISearchResultService searchService, ILogger<SynchronizationController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        [HttpPost("push-visual-assessment")]
        public async Task<IActionResult> PushVisualAssessmentResults(VisualAssessmentResultDto model)
        {
            _logger.LogInformation("Synchronization Endpoint started");
            MessageResponse result = await _searchService.Push(model);
            _logger.LogInformation("Synchronization Endpoint ended");
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("test")]
        public IActionResult GetTest()
        {
            return Ok("Working");
        }

        

    }
}
