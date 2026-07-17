using DVLA.Business.ReportModule;
using DVLA.Data.Models.DataObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [EnableRateLimiting("ExternalOperation")]
    [ApiController]
    [Route("api/[controller]")]
    public class TransmissionsController : ControllerBase
    {
        private readonly IReportRepository _reportRepository;

        public TransmissionsController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] TransmissionGridDto model)
        {
            return Ok(await _reportRepository.FetchDataAsync(model));
        }

        [HttpPost("push-single/{id:long}")]
        public async Task<IActionResult> PushSingleData(long id, [FromQuery] string source, [FromQuery] string destination)
        {
            var result = await _reportRepository.PushDataAsync(id, source, destination);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("push")]
        public async Task<IActionResult> PushData([FromQuery] string source, [FromQuery] string destination)
        {
            var result = await _reportRepository.PushDataAsync(null, source, destination);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
