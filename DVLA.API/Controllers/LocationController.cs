using DVLA.Business.LocationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions()
        {
            return Ok(await _locationService.GetAllRegions());
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts()
        {
            return Ok(await _locationService.GetAllDistricts());
        }

        [HttpGet("countries")]
        public IActionResult GetCountries()
        {
            return Ok(_locationService.GetCountries());
        }

        [HttpGet("districts/by-region/{regionId:int}")]
        public async Task<IActionResult> GetDistrictsByRegion(int regionId)
        {
            return Ok(await _locationService.GetDistrictsByRegion(regionId));
        }

        [HttpGet("districts/by-region-with-facilities/{regionId:int}")]
        public async Task<IActionResult> GetDistrictsByRegionWithFacilities(int regionId)
        {
            return Ok(await _locationService.GetDistrictsByRegionWithFacilities(regionId));
        }
    }
}
