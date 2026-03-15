using System.Threading.Tasks;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AuditLogController : Controller
    {
        private readonly IAuditRepo _auditRepo;

        public AuditLogController(IAuditRepo auditRepo)
        {
            _auditRepo = auditRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ActivityGridViewModel model = new()
            {
                Filter = new()
                {
                    EndDate = DateTime.UtcNow,
                    StartDate = DateTime.UtcNow.AddMonths(-1)
                }
            };
            model.Activities = await _auditRepo.GetAuditAsync(model.Filter);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ActivityGridViewModel model)
        {
            model.Activities = await _auditRepo.GetAuditAsync(model.Filter!);
            return View(model);
        }
    }
}
