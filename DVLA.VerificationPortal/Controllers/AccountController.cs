using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequest model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage);
                return View(model);
            }
            

            ApplicationUser? user = await _userService.GetUserByEmail(model.Email);
            if (user == null)
            {
                model.Errors.Add("Invalid Email/Password");
                return View(model);
            }
            if (user.IsFirstLogin)
            {
                string token = await _userService.GeneratePasswordResetTokenAsync(user);
                return RedirectToAction("ResetPassword", new { id = user.Id, token = token });
            }
            MessageResponse loginResult = await _userService.LoginAsync(model);
            if (loginResult.Success)
            {
                TempData["SuccessMessage"] = loginResult.Message;
                    return RedirectToAction("Index", "Home");
            }
            model.Errors.Add(loginResult.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ApplicationUserDto model = new();
            return View(model);
        }


        [HttpPost]
        
        public async Task<IActionResult> Register(OnboardUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage);
                return View(model);
            }
            var onboardUserResult = await _userService.OnboardUserAsync(model);
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            ForgotPasswordRequest model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            MessageResponse response = await _userService.SendResetPasswordTokenAsync(model);
            if (response.Success)
            {
                //Call Notification Service
                TempData["SuccessMessage"] = response.Message;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string encodedToken, string userid)
        {
            var result = await _userService.ConfirmEmail(encodedToken, userid);
            if (result)
            {
                TempData["SuccessMessage"] = "Your account has been successfully activated.";
                return RedirectToAction("Login");
            }
            return View("Error");
        }


        [HttpGet]
        public IActionResult ResetPassword(string id, string token)
        {
            ResetPasswordRequest model = new() { Id = id, ResetToken = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage);
                return View(model);
            }
            var resetPasswordResult = await _userService.ResetPasswordAsync(model);
            if (resetPasswordResult.Success)
            {
                TempData["SuccessMessage"] = resetPasswordResult.Message;
                return RedirectToAction("Login");
            }
            model.Errors.Add(resetPasswordResult.Message);
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ChangePasswordRequest request = new();
            return View(request);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            string email = HttpContext.User.Identity.Name;
            if (!ModelState.IsValid)
            {
                request.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                return View(request);
            }
            MessageResponse response = await _userService.ChangePasswordAsync(request);
            if(response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index", "Home");
            }
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return RedirectToAction("Login");
        }



    }
}
