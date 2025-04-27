using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DVLA.Business.ReportModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TransmissionController : Controller
    {
        private readonly IReportRepository _reportService;

        public TransmissionController(IReportRepository reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            TransmissionGridDto model = new ();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(TransmissionGridDto model)
        {
            model = await _reportService.FetchDataAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> PushSingleData(long id, string source, string destination)
        {
            MessageResponse result = await _reportService.PushDataAsync(id, source, destination);
            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> PushData(string source, string destination)
        {
            MessageResponse result = await _reportService.PushDataAsync(null, source, destination);
            return Json(result);
        }
    }
}
