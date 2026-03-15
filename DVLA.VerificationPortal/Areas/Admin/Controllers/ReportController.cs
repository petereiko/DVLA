using System.Threading.Tasks;
using DVLA.VerificationPortal.Controllers;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator, Super Admin")]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;
        private readonly IAuditRepo _auditRepo;

        public ReportController(IReportService reportService, IAuditRepo auditRepo):base(auditRepo)
        {
            _reportService = reportService;
            _auditRepo = auditRepo;
        }

        [HttpGet]
        public IActionResult GetResults()
        {
            TestResultCountGridViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetResults(TestResultCountGridViewModel model)
        {
            model.Results = await _reportService.GetResults(model.StartDate, model.EndDate, model.PassOrFail);
            await LogAuditAsync("Fetched Results");
            return View(model);
        }


        [HttpGet]
        public IActionResult GetVerifiedResults()
        {
            VerifiedItemGridViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetVerifiedResults(VerifiedItemGridViewModel model)
        {
            model.Results = await _reportService.GetVerifiedResults(model.StartDate, model.EndDate);
            await LogAuditAsync("Fetched Verified Results");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> VerifiedResultsByUser(string token)
        {
            IEnumerable<TestResultDto> results = await _reportService.VerifiedResultsByUser(token);
            await LogAuditAsync("Fetched Verified Results");
            return View(results);
        }
    }
}
