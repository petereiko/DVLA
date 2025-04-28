using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Areas.Admin.Controllers
{
    public class PinController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
