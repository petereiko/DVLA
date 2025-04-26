using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DVLA.Business.ReportModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.UI.Areas.Admin.Controllers
{
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
            List<VisualAssessmentResultDto> results =new ();
            return View(results);
        }

        [HttpPost]
        public IActionResult Index(TransmissionRequestDto model)
        {
            var results = _reportService.FetchData(model);
            return View(results);
        }

        [HttpGet]
        public async Task<JsonResult> PushData()
        {
            MessageResponse result = await _reportService.PushData();
            return Json(result);
        }
    }
}
