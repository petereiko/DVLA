using DVLA.VerificationPortal.Infrastructure;
using DVLA.VerificationPortal.Infrastructure.Models;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Models;
using DVLA.VerificationPortal.Shared;
using DVLA.VerificationPortal.Shared.DTOs;
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
        private readonly IAuditRepo _auditRepo;
        private readonly IUserRepository _userRepository;


        public HomeController(ILogger<HomeController> logger, ISearchResultService searchService, IAuditRepo auditRepo, IUserRepository userRepository) : base(auditRepo)
        {
            _logger = logger;
            _searchService = searchService;
            _auditRepo = auditRepo;
            _userRepository = userRepository;
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
            VisualAssessmentResultDto result = await _searchService.GetAssessmentResultAsync(key);
            await LogAuditAsync("Fetch Visual Assessment Details");
            return View(result);
        }

        [HttpGet]
        public async Task<JsonResult> VerifyResult(string token)
        {
            MessageResponse<string> response = new();

            ApplicationUserDto applicationUser = await _userRepository.GetUserByEmail(HttpContext.User.Identity.Name);
            List<string> roles = await _userRepository.GetRolesAsync(applicationUser);
            if (!roles.Contains(EnumHelper.GetEnumDescription(Role.Verifier)))
            {
                response.Message = "You are not a Verifier";
                return Json(response);
            }
            response = await _searchService.VerifyResultByReferenceAsync(token, VerifyType.WEB);
            return Json(response);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerifiedResults()
        {
            var results = await _searchService.GetVerifiedResultsAsync();
            return View(results);
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
