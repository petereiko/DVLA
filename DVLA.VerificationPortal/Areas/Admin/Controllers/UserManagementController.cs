using AutoMapper;
using Azure.Core;
using DVLA.VerificationPortal.Application.Interfaces;
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
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserManagementController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10)
        {
            PaginatedResponse<ApplicationUserDto> response = await _userService.GetAllAsync(pageIndex, pageSize);
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
                return View(model);
            }

            ApplicationUserDto userDto = await _userService.GetUserByEmailAsync(model.Email);

            if (userDto != null)
            {
                model.Errors.Add("Email Address already exist");
                return View(model);
            }

            userDto = await _userService.OnboardUserAsync(model);



            TempData["SuccessMessage"] = "Record saved successfully";
            //_AuditRepo.AddAudit(Activities.CREATE_USER, "Added User Details");
            return RedirectToAction("Index");

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
            ApplicationUserDto user = await _userService.UpdateAsync(request);
            if (user != null)
            {
                TempData["SuccessMessage"] = "Record saved successfully";
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
