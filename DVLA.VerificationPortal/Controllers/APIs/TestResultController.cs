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
    public class TestResultController : BaseApiController
    {
        private readonly ISearchResultService _searchResultService;


        public TestResultController(ISearchResultService searchResultService, IApiClientService apiClientService):base(apiClientService)
        {
            _searchResultService = searchResultService;
        }


        [HttpGet("get-test/{reference}")]
        public async Task<IActionResult> GetTest(string reference)
        {
            await AuditLogAsync();

            TestResultDto? result = await _searchResultService.GetResultAsync(reference);
            if (result == null)
            {
                return BadRequest(new { status = "error", message = "Applicant not found" });
            }
            //if (result.Verified)
            //{
            //    return BadRequest(new { status = "error", message = "Test has already been verified once" });
            //}
            return Ok(new { status = "success", data = result });
        }

        [HttpGet("verify-result/{reference}")]
        public async Task<IActionResult> VerifyResult(string reference)
        {
            await AuditLogAsync();

            MessageResponse response = await _searchResultService.VerifyResultByReferenceAsync(reference);
            return Ok(response);
        }
    }
}
