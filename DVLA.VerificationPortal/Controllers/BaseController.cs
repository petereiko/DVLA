using DVLA.VerificationPortal.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Controllers
{
    public class BaseController : Controller
    {
        private readonly IAuditRepo _auditRepo;

        public BaseController(IAuditRepo auditRepo)
        {
            _auditRepo = auditRepo;
        }

        public async Task LogAuditAsync(string description)
        {
            await _auditRepo.AddAuditAsync(this.ControllerContext.ActionDescriptor.ActionName, description);
        }
    }
}
