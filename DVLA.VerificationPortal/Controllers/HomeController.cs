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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISearchResultService _searchService;
        private readonly IAuthUser _authUser;

        public HomeController(ILogger<HomeController> logger, ISearchResultService searchService, IAuthUser authUser)
        {
            _logger = logger;
            _searchService = searchService;
            _authUser = authUser;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<PartialViewResult> SearchResults(string searchTerm)
        {
            var results = await _searchService.GetResultsAsync(searchTerm);
            return PartialView("~/Views/Shared/_Result.cshtml", results);
        }

        [HttpGet]
        public async Task<ActionResult> Details(string key)
        {
            int id = Utility.DecryptUrlID(key);
            var result = await _searchService.GetResultAsync(id);
            return View(result);
        }

        [HttpGet]
        public async Task<JsonResult> VerifyResult(string token)
        {
            MessageResponse response = new();
            if (_authUser.Role != EnumHelper.GetEnumDescription(Role.Verifier))
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
