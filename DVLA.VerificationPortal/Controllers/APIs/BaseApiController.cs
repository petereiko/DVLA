using System.Diagnostics;
using DVLA.VerificationPortal.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DVLA.VerificationPortal.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private readonly IApiClientService _clientService;

        protected string CurrentController => ControllerContext.RouteData.Values["controller"]?.ToString() ?? "Unknown";
        protected string CurrentAction => ControllerContext.RouteData.Values["action"]?.ToString() ?? "Unknown";

        public BaseApiController(IApiClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task AuditLogAsync()
        {

            await _clientService.AuditLogAsync(CurrentController, CurrentAction, _clientService.ApiId);
        }
    }
}
