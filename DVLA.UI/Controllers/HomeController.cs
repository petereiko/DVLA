using DVLA.Business.BackgroundJobModule;
using DVLA.Business.SlotModule;
using DVLA.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DVLA.UI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISlotUsageRepository _slotUsageRepository;
        

        public HomeController(ILogger<HomeController> logger, ISlotUsageRepository slotUsageRepository)
        {
            _logger = logger;
            _slotUsageRepository = slotUsageRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetTotalSlot()
        {
            return Json(await _slotUsageRepository.GetTotalSlots());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
