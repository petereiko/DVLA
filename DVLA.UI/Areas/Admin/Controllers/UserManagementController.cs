using DVLA.Business.NotificationModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.SYSTEMADMIN)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepositoryQuery<ApplicationUser> _userRepositoryQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepositoryQuery<OptometristFirm> _optometristQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IConfiguration _configuration;
        private readonly IAuditRepo _AuditRepo;
        private readonly ILogger<UserManagementController> _logger;
        private readonly DVLADbContext _context;
        private readonly IAuthUser _authUser;

        public UserManagementController(IRepositoryQuery<ApplicationUser> userRepositoryQuery
            , INotificationRepository notificationRepository, IUserService userService, IAuditRepo AuditRepo, IRepositoryQuery<OptometristFirm> optometristQuery, IRepositoryQuery<OptometristFirmUser> optometristUserQuery, IUserRepository userRepository, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, ILogger<UserManagementController> logger, DVLADbContext context, IAuthUser authUser)
        {
            _userRepository = userRepository;
            _optometristQuery = optometristQuery;
            _optometristUserQuery = optometristUserQuery;
            _userRepositoryQuery = userRepositoryQuery;
            _notificationRepository = notificationRepository;
            _AuditRepo = AuditRepo;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _context = context;
            _authUser = authUser;
        }
        // GET: User
        public ActionResult Index()
        {
            List<UserViewModel> users = new ();
            if (User.IsInRole(AppRoles.OPTOMETRIST) || User.IsInRole(AppRoles.FACILITYOWNER))
            {
                users = _userRepository.GetUsers(null, _authUser.UserId);
            }
            else
            {
                users = _userRepository.GetUsers(null, _authUser.UserId);
            }
            
            _AuditRepo.AddAudit(Activities.VIEW_USER, "View Users");
            return View(users);
        }


        public async Task<ActionResult> Create()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            UserViewModel model = new();
            model.Roles = await _roleManager.Roles.AsNoTracking().Select(x => new CheckBoxListItemDto { Id = x.Id, IsChecked = false, Name = x.Name }).ToListAsync();
            ViewBag.OptometristFirms = _optometristQuery.GetAll().OrderBy(x => x.BusinessName).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            try
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                ViewBag.OptometristFirms = _optometristQuery.GetAll().OrderBy(x => x.BusinessName).ToList();
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var emailAddress = await _userManager.FindByEmailAsync(model.Email);

                if (emailAddress != null)
                {
                    model.Errors.Add("Email Address already exist");
                    return View(model);
                }

                var context = _context;
                var scope = await context.Database.BeginTransactionAsync();

                using (scope)
                {
                    if(model.Roles.Where(x=>x.IsChecked).Select(x=>x.Name).Contains(AppRoles.OPTOMETRIST) && string.IsNullOrEmpty(model.PIN))
                    {
                        scope.Rollback();
                        model.Errors.Add("You must enter Personal Identification Number");
                        return View(model);
                    }
                    ApplicationUser applicationUser = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Email.Trim().ToLower() == model.Email.ToLower().Trim());
                    if (applicationUser != null)
                    {
                        scope.Rollback();
                        model.Errors.Add("User with the email already exist");
                        return View(model);
                    }
                    if (model.Roles.Any(x => x.IsChecked && x.Name.Contains(AppRoles.OPTOMETRIST)))
                    {
                        applicationUser = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Pin.Trim().ToLower() == model.PIN.ToLower().Trim());
                        if (applicationUser != null)
                        {
                            scope.Rollback();
                            model.Errors.Add("User with the Pin already exist");
                            return View(model);
                        }
                    }
                    applicationUser = new ()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsActive = true,
                        MobileNumber = model.MobileNumber,
                        UserName = model.Email,
                        CreatedBy = _authUser.UserId,
                        Pin = model.PIN,
                        EmailConfirmed = model.EmailConfirmed,
                        PhoneNumber = model.Phone,
                        IsDeleted = false
                    };

                    var pwd = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);

                    var user = await _userManager.CreateAsync(applicationUser, pwd);

                    if (user.Succeeded)
                    {
                        if (model.OptometristFirmId > 0 && (model.Roles.Where(x=>x.IsChecked).Any(r=>r.Name != AppRoles.SYSTEMADMIN || r.Name != AppRoles.SLOTMANAGER)))
                        {
                            _optometristUserQuery.Add(new OptometristFirmUser()
                            {
                                OptometristFirmId = (int)model.OptometristFirmId,
                                ApplicationUserId = applicationUser.Id
                            });

                            applicationUser.OptometristFirmId = model.OptometristFirmId.GetValueOrDefault();
                            await _userManager.UpdateAsync(applicationUser);
                        }

                        //var userId = applicationUser.Id;
                        foreach (var item in model.Roles.Where(x=>x.IsChecked))
                        {
                            await _userManager.AddToRoleAsync(applicationUser, item.Name);
                        }
                        

                        //var code = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);

                        //var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = applicationUser.Id, code = code });
                        //string mPre = _configuration["AppConstants:BaseUrl"] + callbackUrl;

                        _notificationRepository.SendNewAccountCreated(applicationUser, pwd);
                        await scope.CommitAsync();
                    }
                    else
                    {
                        await scope.RollbackAsync();
                        model.Errors.Add("Unable to save record");
                        return View(model);
                    }
                }

                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.CREATE_USER, "Added User Details");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }


        public ActionResult Update(string Id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            ViewBag.Roles = _roleManager.Roles.ToList();
            ViewBag.OptometristFirms = _optometristQuery.GetAll().OrderBy(x => x.BusinessName).ToList();
            UserViewModel model = _userRepository.GetUserDetails(Id);
            model.Id = Id;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(UserViewModel model, string Id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            
            try
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                ViewBag.OptometristFirms = _optometristQuery.GetAll().OrderBy(x => x.BusinessName).ToList();
                model.Id = Id;
                if (!ModelState.IsValid)
                {
                    model.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return View(model);
                }
                var updateResult = await _userRepository.UpdateAsync(model);
                if (updateResult.Success)
                {
                    TempData["SuccessMessage"] =updateResult.Message;
                    _AuditRepo.AddAudit(Activities.UPDATE_USER, "Update User Details");
                    return RedirectToAction("Index");
                }
                else
                {
                    model.Errors.Add(updateResult.Message);
                    return View(model);
                 }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                model.Errors.Add("Kindly try again later");
                return View(model);
            }
        }



        [HttpPost]
        public async Task<ActionResult> ResetPassword(string Id)
        {
            string message = "";
            try
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user == null)
                {
                    return Json(new { success = true, message = "User not Found" });
                }

                var code = _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, area = "" });
                string mPre = _configuration["AppConstants:BaseUrl"] + callbackUrl;

                _notificationRepository.SendPasswordReset(user, mPre);

                message = "Password for User " + user.FullName + " has been reset successfully";

                string mDesc = $"Password for User {user.FullName} has been reset successfully";
                _AuditRepo.AddAudit(Activities.RESET_PASSWORD, "Reset User Password");
                return Json(new { success = true, message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Json(new { success = false, message = "Kindly try again later" });
            }
        }
        [HttpPost]
        public async Task<JsonResult> ChangeStatus(string Id)
        {
            string message = "";
            try
            {
                var applicationUser = await _userManager.FindByIdAsync(Id);
                if (applicationUser.IsActive)
                {
                    message = "User " + applicationUser.FullName + " has been deactivated successfully";
                    applicationUser.IsActive = false;
                    applicationUser.IsDeleted = true;
                }
                else
                {
                    message = "User " + applicationUser.FullName + " has been activated successfully";
                    applicationUser.IsActive = true;
                    applicationUser.IsDeleted = false;
                }

                applicationUser.ModifiedBy = _authUser.UserId;
                applicationUser.DateUpdated = DateTime.Now;
                var updateUser = await _userManager.UpdateAsync(applicationUser);
                if (updateUser.Succeeded)
                {
                    return Json(new { success = true, message });
                }
                else
                {
                    return Json(new { success = false, message = "Unable to change user status" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Json(new { success = false, message = "Kindly try again later" });
            }
        }
    }

}