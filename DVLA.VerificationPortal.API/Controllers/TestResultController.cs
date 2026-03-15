using DVLA.VerificationPortal.API.Security;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.API.Controllers
{
    [ApiKey]
    [Route("api/[controller]")]
    [ApiController]
    
    public class TestResultController : ControllerBase
    {
        private readonly ISearchResultService _searchResultService;

        public TestResultController(ISearchResultService searchResultService)
        {
            _searchResultService = searchResultService;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Ok("Working");
        }

        [HttpGet("get-test/{reference}")]
        public async Task<IActionResult> GetTest(string reference)
        {
            TestResultDto? result = await _searchResultService.GetResultAsync(reference);
            if (result == null)
            {
                return BadRequest(new { status = "error", message = "Applicant not found" });
            }
            if (result.Verified)
            {
                return BadRequest(new { status = "error", message = "Test has already been verified once" });
            }
            return Ok(new { status = "success", data = new { FullName = result.FullName, PassConclusion = result.PassConclusion } });
        }
    }
}
