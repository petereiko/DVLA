using DVLA.Business.OptometristFirmModule;
using DVLA.Business.ReportModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OptometristFirmsController : ControllerBase
    {
        private readonly IOptometristService _optometristService;
        private readonly IReportRepository _reportRepository;

        public OptometristFirmsController(IOptometristService optometristService, IReportRepository reportRepository)
        {
            _optometristService = optometristService;
            _reportRepository = reportRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationRequestModel model)
        {
            model ??= new PaginationRequestModel();
            return Ok(await _optometristService.GetAllOptometricFirms(model));
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup([FromQuery] int? regionId, [FromQuery] int? districtId)
        {
            return Ok(await _reportRepository.FetchAllOptometristFirms(regionId, districtId));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _optometristService.GetOptometricFirm(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OptometristFirmViewModel model)
        {
            var result = await _optometristService.CreateOptometricFirm(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] OptometristFirmViewModel model)
        {
            model.Id = id;
            var result = await _optometristService.UpdateOptometricFirm(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:int}/change-status")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _optometristService.ChangeStatus(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
