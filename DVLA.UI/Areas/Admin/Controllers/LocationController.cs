using DVLA.Business.LocationModule;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<JsonResult> GetDistrictsByRegion(int regionId)
        {
            var districts = await _locationService.GetDistrictsByRegion(regionId);
            return Json(districts);
        }
    }
}
