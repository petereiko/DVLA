using AutoMapper;
using Azure.Core;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Controllers;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using DVLA.VerificationPortal.Shared.Requests;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Super Admin")]
    public class UserManagementController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserManagementController(IUserService userService, IMapper mapper, IAuditRepo auditRepo):base(auditRepo)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            PaginatedResponse<ApplicationUserDto> response = await _userService.GetAllAsync(pageIndex, pageSize);
            await LogAuditAsync("Fetched Users");
            return View(response);
        }

        [HttpGet]
        public IActionResult Create()
        {
            OnboardUserRequest request = new();
            request.Roles = _userService.GetAllRoles().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
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
            model.Roles = _userService.GetAllRoles().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();

            if (!ModelState.IsValid)
            {
                model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                return View(model);
            }

            ApplicationUserDto userDto = await _userService.GetUserByEmailAsync(model.Email!);

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
            ApplicationUserDto user = await _userService.GetUserByIdAsync(id);
            EditUserRequest request = _mapper.Map<EditUserRequest>(user);
            request.Roles = _userService.GetAllRoles().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
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

            EditUserRequest request = _mapper.Map<EditUserRequest>(model);
            MessageResponse response = await _userService.UpdateAsync(request);
            if (response.Success)
            {
                await LogAuditAsync("Edited Users");
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            model.Roles = _userService.GetAllRoles().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Text = x.Name,
                Value = x.Id
            }).OrderBy(x => x.Text).ToList();
            return View(model);
        }
    }
}
