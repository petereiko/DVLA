using DVLA.Business.VisualAssessmentResultModule;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.UI.Areas.Admin.Controllers
{
    public class TransmissionController : Controller
    {
        private readonly IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        public IActionResult Index()
        {
            return View();
        }
    }
}
