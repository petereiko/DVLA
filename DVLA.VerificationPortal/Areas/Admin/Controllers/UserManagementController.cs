using Azure;
using Azure.Core;
using DVLA.VerificationPortal.Controllers;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super Admin")]
    public class UserManagementController : BaseController
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService, IAuditRepo auditRepo):base(auditRepo)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 1000)
        {
            PaginatedResponse<ApplicationUserDto> response = await _userService.GetAllAsync(pageIndex, pageSize);
            await LogAuditAsync("Fetched Users");
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            OnboardUserRequest request = new();
            request.Roles = (await _userService.GetAllRoles()).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();
            return View(request);
        }


        [HttpPost]
        public async Task<ActionResult> Create(OnboardUserRequest model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            model.Roles = (await _userService.GetAllRoles()).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();

            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                return View(model);
            }

            ApplicationUser? userDto = await _userService.GetUserByEmail(model.Email!);

            if (userDto != null)
            {
                model.Errors.Add("Email Address already exist");
                return View(model);
            }

            MessageResponse response = await _userService.OnboardUserAsync(model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Record saved successfully";
                //_AuditRepo.AddAudit(Activities.CREATE_USER, "Added User Details");
                await LogAuditAsync("Created Users");
                return RedirectToAction("Index");
            }
            model.Errors.Add(response.Message);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userService.GetUserByIdAsync(id);

            IList<string> roles = await _userService.GetUserRoles(user);

            EditUserRequest request = new()
            {
                Email = user.Email,
                CentreName = user.CentreName,
                EmailConfirmed = user.EmailConfirmed,
                Role = roles.FirstOrDefault()!,
                Id = id,
                IsActive = user.IsActive,
                PhoneNumber = user.PhoneNumber
            };
            request.Roles = (await _userService.GetAllRoles()).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();
            request.Role = request.Roles.FirstOrDefault(x => x.Text == request.Role)?.Value;
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                return View(model);
            }

            //EditUserRequest request = _mapper.Map<EditUserRequest>(model);
            MessageResponse response = await _userService.EditUser(model);
            if (response.Success)
            {
                await LogAuditAsync("Edited Users");
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            model.Roles = (await _userService.GetAllRoles()).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            ResetPasswordRequest model = new ResetPasswordRequest();
            var user = await _userService.GetUserByIdAsync(id);
            model.ResetToken = await _userService.GeneratePasswordResetTokenAsync(user);
            model.Id = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
        {
            model.ConfirmPassword = model.Password;
            MessageResponse result = await _userService.ResetPasswordAsync(model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction("Index");
        }
    }
}
