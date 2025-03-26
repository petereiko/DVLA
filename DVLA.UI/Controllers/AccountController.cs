using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DVLA.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(IUserService userService, ILogger<AccountController> logger, RoleManager<ApplicationRole> roleManager)
        {
            _userService = userService;
            _logger = logger;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage);
                return View(model);
            }

            UserViewModel userModel = await _userService.GetUserByEmail(model.Email);
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
            MessageResponse<UserViewModel> loginResult = await _userService.Login(model);
            if (loginResult.Success)
            {
                TempData["SuccessMessage"] = "Login Successful";
                    return RedirectToAction("Index", "Dashboard");
            }
            model.Errors.Add(loginResult.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            UserViewModel model = new();
            return View(model);
        }


        [HttpPost]
        
        public async Task<IActionResult> Register(UserViewModel model)
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
            ForgotPasswordViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            MessageResponse response = await _userService.SendResetPasswordToken(model);
            if (response.Success)
            {
                model.SuccessMessage = response.Message;
            }
            else
            {
                model.ErrorMessage = response.Message;
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
            ResetPasswordViewModel model = new() { Id = id, ResetToken = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
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
