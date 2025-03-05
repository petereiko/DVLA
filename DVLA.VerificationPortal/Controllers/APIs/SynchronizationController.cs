using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class SynchronizationController : ControllerBase
    {
        private readonly ISearchResultService _searchService;

        public SynchronizationController(ISearchResultService searchService)
        {
            _searchService = searchService;
        }

        [HttpPost("push-visual-assessment")]
        public async Task<IActionResult> PushVisualAssessmentResults()
        {
            MessageResponse result = await _searchService.Push();
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }

        

    }
}
