using DVLA.VerificationPortal.CustomAttributes;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Models;
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
        private IApiClientService _apiClientService;
        private readonly IReportService _reportService;

        public TestResultController(ISearchResultService searchResultService, IApiClientService apiClientService, IReportService reportService) : base(apiClientService)
        {
            _searchResultService = searchResultService;
            _apiClientService = apiClientService;
            _reportService = reportService;
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
            var apiClient = _apiClientService;
            var response = await _searchResultService.VerifyResultByReferenceAsync(reference, Shared.Enums.VerifyType.API);
            return Ok(new { resultConclusion = response.Result, success = response.Success, message = response.Message });
        }

        
    }
}
