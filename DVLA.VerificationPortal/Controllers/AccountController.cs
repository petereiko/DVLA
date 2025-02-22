using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserRepository userService, ILogger<AccountController> logger)
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
            

            ApplicationUserDto userModel = await _userService.GetUserByEmail(model.Email);
            if (userModel == null)
            {
                model.Errors.Add("Invalid Email/Password");
                return View(model);
            }
            if (userModel.IsFirstLogin)
            {
                string token = await _userService.GeneratePasswordResetToken(userModel.Id);
                return RedirectToAction("ResetPassword", new { id = userModel.Id, token = token });
            }
            MessageResponse<ApplicationUserDto> loginResult = await _userService.Login(model);
            if (loginResult.Success)
            {
                TempData["SuccessMessage"] = "Login Successful";
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
            var onboardUserResult = await _userService.OnboardUser(model);
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
            MessageResponse<string> response = await _userService.SendResetPasswordToken(model);
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
            var resetPasswordResult = await _userService.ResetPassword(model);
            if (resetPasswordResult.Success)
            {
                TempData["SuccessMessage"] = resetPasswordResult.Message;
                return RedirectToAction("Login");
            }
            model.Errors.Add(resetPasswordResult.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();
            return RedirectToAction("Login");
        }



    }
}
