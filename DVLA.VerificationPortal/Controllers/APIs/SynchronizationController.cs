using DVLA.VerificationPortal.CustomAttributes;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKey]
    public class SynchronizationController(
        ISearchResultService searchService,
        ILogger<SynchronizationController> logger,
        IOptometristFirmSynchronization optometristFirmSyncService)
        : ControllerBase
    {
        [HttpPost("push-visual-assessment")]
        public async Task<IActionResult> PushVisualAssessmentResults(VisualAssessmentResultDto model)
        {
            logger.LogInformation("Synchronization Endpoint started");
            MessageResponse result = await searchService.Push(model);
            logger.LogInformation("Synchronization Endpoint ended");
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpPost("update-auth-doc")]
        public async Task<IActionResult> UpdateAuthDoctor(UpdateDocRequestDto model)
        {
            //_logger.LogInformation("Synchronization Endpoint started");
            MessageResponse result = await searchService.UpdateAuthDoctor(model);
            //_logger.LogInformation("Synchronization Endpoint ended");
            if (result.Success)
                return Ok(result);
            return BadRequest(result.Message);
        }

        [HttpGet("test")]
        public IActionResult GetTest()
        {
            return Ok("Working");
        }


        [HttpPost("sync-optometrist-firm")]
        public async Task<IActionResult> SyncOptometristFirm([FromBody] OptometristFirm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            MessageResponse response = await optometristFirmSyncService.SyncOptometristFirm(model);
            return Ok(response);
        }


        [HttpPost("sync-optometrist-firms")]
        public async Task<IActionResult> SyncOptometristFirms([FromBody] List<OptometristFirm> model)
        {
            List<int> response = await optometristFirmSyncService.SyncOptometristFirms(model);
            return Ok(response);
        }



    }
}
