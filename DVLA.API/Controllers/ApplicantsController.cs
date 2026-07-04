using DVLA.Business.ApplicantModule;
using DVLA.Data.Models.DataObjects.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicantsController : ControllerBase
    {
        private readonly IApplicantService _applicantService;

        public ApplicantsController(IApplicantService applicantService)
        {
            _applicantService = applicantService;
        }

        [HttpGet("{id:long}")]
        public IActionResult Get(long id)
        {
            var result = _applicantService.Get(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(string id, [FromBody] ApplicantModel model)
        {
            var result = _applicantService.Update(model, id);
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
