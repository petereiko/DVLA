using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Models;
using DVLA.VerificationPortal.Shared;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DVLA.VerificationPortal.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISearchResultService _searchService;
        private readonly IAuthUser _authUser;
        private readonly IAuditRepo _auditRepo;

        public HomeController(ILogger<HomeController> logger, ISearchResultService searchService, IAuthUser authUser, IAuditRepo auditRepo) : base(auditRepo)
        {
            _logger = logger;
            _searchService = searchService;
            _authUser = authUser;
            _auditRepo = auditRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> SearchResults(string searchTerm)
        {
            var results = await _searchService.GetResultsAsync(searchTerm);
            await LogAuditAsync("Search Visual Assessment Results");
            return PartialView("~/Views/Shared/_Result.cshtml", results);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string key)
        {
            int id = Utility.DecryptUrlID(key);
            var result = await _searchService.GetResultAsync(id);
            await LogAuditAsync("Fetch Visual Assessment Details");
            return View(result);
        }

        [HttpGet]
        public async Task<JsonResult> VerifyResult(string token)
        {
            MessageResponse response = new();
            if (_authUser.Role != EnumHelper.GetEnumDescription(Role.Verifier))//274556

            {
                response.Message = "You are not a Verifier";
                return Json(response);
            }
            response = await _searchService.VerifyResult(token, VerifyType.WEB);
            
            return Json(response);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            // Retrieve the error message from HttpContext.Items
            if (HttpContext.Items.TryGetValue("ErrorMessage", out var errorMessage))
            {
                ViewBag.ErrorMessage = errorMessage;
            }
            else
            {
                ViewBag.ErrorMessage = "An unexpected error occurred.";
            }

            return View();
        }
    }
}
