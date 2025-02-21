
using DVLA.Business.NotificationModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Authorize(Roles = AppRoles.FACILITYOWNER)]
    [Area("Customer")]
    public class UserManagementController : Controller
    {
        private readonly IRepositoryQuery<ApplicationUser> _userRepositoryQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRepositoryQuery<OptometristFirm> _optometristQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IAuditRepo _AuditRepo;
        private readonly IUserService _userService;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string currentUserId;
        private readonly ILogger<UserManagementController> _logger;
        private readonly DVLADbContext _context;
        private readonly IConfiguration _configuration;

        public UserManagementController(IRepositoryQuery<ApplicationUser> userRepositoryQuery
            , INotificationRepository notificationRepository, IAuditRepo AuditRepo, IRepositoryQuery<OptometristFirm> optometristQuery, IRepositoryQuery<OptometristFirmUser> optometristUserQuery, IUserRepository userRepository, IUserService userService, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<UserManagementController> logger, DVLADbContext context)
        {
            _userRepository = userRepository;
            _optometristQuery = optometristQuery;
            _optometristUserQuery = optometristUserQuery;
            _userRepositoryQuery = userRepositoryQuery;
            _notificationRepository = notificationRepository;
            _AuditRepo = AuditRepo;
            _userService = userService;
            _roleManager = roleManager;
            _userManager = userManager;
            currentUserId = userService.GetUserData().Id;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }
        // GET: User
        public ActionResult Index()
        {
            List<UserViewModel> users = new List<UserViewModel>();
            if (User.IsInRole(AppRoles.FACILITYOWNER))
            {
                var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
                int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
                users = _userRepository.GetUsersByOptometristFirm(OptometristFirmId);
                users = users.Where(x => x.Id != currentUserId).ToList();
            }
            _AuditRepo.AddAudit(Activities.VIEW_USER, "View Users");
            return View(users);
        }


        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            ViewBag.Roles = _roleManager.Roles.Where(x => x.Name == AppRoles.FRONTOFFICER).ToList();
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var firm = _optometristQuery.Filter(x => x.Id == OptometristFirmId).FirstOrDefault().BusinessName;            
            return View(new UserViewModel { OptometristFirmId = OptometristFirmId, OptometristFirmName = firm});
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Customer", new { area = "Customer" });
            }
            //ViewBag.Optometrists = _optometristQuery.GetAll().ToList();

            try
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                ViewBag.Optometrists = _optometristQuery.GetAllAsync().GetAwaiter().GetResult().ToList();

                if (!ModelState.IsValid)
                {
                    model.Errors.Add(ModelState.Values.SelectMany(x => x.Errors).FirstOrDefault()?.ErrorMessage);
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.PIN))
                {
                    model.Errors.Add("PIN is required");
                    return View(model);
                }

                var emailAddress = await _userManager.FindByEmailAsync(model.Email);

                if (emailAddress != null)
                {
                   model.Errors.Add("Email Address already exist");
                    return View(model);
                }

                var context = _context;
                var transaction = await context.Database.BeginTransactionAsync();

                using (transaction)
                {
                    var applicationUser = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Pin == model.PIN.Trim());
                    if(applicationUser != null) 
                    {
                        await transaction.RollbackAsync();
                        model.Errors.Add("PIN is in use");
                        return View(model);
                    }
                    applicationUser = new ApplicationUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        //OptometristFirmId = OptometristFirmId,
                        IsActive = true,
                        MobileNumber = model.MobileNumber,
                        UserName = model.Email,
                        CreatedBy = currentUserId,
                        EmailConfirmed = true,
                        DefaultRole = AppRoles.FRONTOFFICER,
                        IsDeleted = false,
                        PhoneNumber = model.Phone,
                        Pin = model.PIN
                    };

                    var pwd = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);

                    var user = await _userManager.CreateAsync(applicationUser, pwd);

                    if (user.Succeeded)
                    {
                        if (model.OptometristFirmId != null)
                        {
                            await _optometristUserQuery.AddAsync(new OptometristFirmUser()
                            {
                                OptometristFirmId = (int)model.OptometristFirmId,
                                ApplicationUserId = applicationUser.Id
                            });
                        }

                        var userId = applicationUser.Id;

                        await _userManager.AddToRoleAsync(applicationUser, model.DefaultRole);

                        var code = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);

                        var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = userId, code = code });
                        string mPre = $"{_configuration["AppConstants:BaseUrl"]}/{callbackUrl}";

                        _notificationRepository.SendNewAccountCreated(applicationUser, pwd, mPre, context);
                        await transaction.CommitAsync();

                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        model.Errors.Add("Unable to save record");
                        return View(model);
                    }
                }

                TempData["SuccesMessage"] =  "Record saved successfully";
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
                return RedirectToAction("Index", "Customer", new { area = "Customer" });
            }
            ViewBag.Roles = _roleManager.Roles.Where(x => x.Name == AppRoles.FRONTOFFICER).ToList();
            //ViewBag.Optometrists = _optometristQuery.GetAll().ToList();
            var model = _userRepository.GetUserDetails(Id);
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            var firm = _optometristQuery.FilterAsync(x => x.Id == OptometristFirmId).GetAwaiter().GetResult().FirstOrDefault().BusinessName;
            model.Id = Id;
            model.OptometristFirmName = firm;
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(UserViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Customer", new { area = "Customer" });
            }

            try
            {
                ViewBag.Roles = _roleManager.Roles.ToList();
                //ViewBag.Optometrists = _optometristQuery.GetAll().ToList();
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                string responseMessage = "";
                bool result = _userRepository.Update(model, currentUserId, out responseMessage);
                if (result)
                {
                    TempData["SuccessMessage"] = responseMessage;
                    _AuditRepo.AddAudit(Activities.UPDATE_USER, "Update User Details");
                    return RedirectToAction("Index");
                }
                else
                {
                   model.Errors.Add(responseMessage);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }




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

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code, area = "" });
                string mPre = $"{_configuration["AppConstants:BaseUrl"]}/{callbackUrl}";

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

        public async Task<JsonResult> ChangeStatus(string Id)
        {
            string message = "";
            try
            {
                var applicationUser = await _userManager.FindByIdAsync(Id);
                if (applicationUser.IsActive)
                {
                    message = "Password for User " + applicationUser.FullName + " has been deactivated successfully";
                    applicationUser.IsActive = false;
                    applicationUser.IsDeleted = true;
                }
                else
                {
                    message = "Password for User " + applicationUser.FullName + " has been activated successfully";
                    applicationUser.IsActive = true;
                    applicationUser.IsDeleted = false;
                }

                applicationUser.ModifiedBy = currentUserId;
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