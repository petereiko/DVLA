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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = AppRoles.FACILITYOWNER)]
    public class OptometristManagementController : Controller
    {
        private readonly IRepositoryQuery<ApplicationUser> _userRepositoryQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly IActivityLogRepositoryCommand _activityLogRepositoryCommand;
        private readonly IRepositoryQuery<OptometristFirm> _optometristQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IAuditRepo _AuditRepo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OptometristManagementController> _logger;
        private readonly string currentUserId;

        public OptometristManagementController(IRepositoryQuery<ApplicationUser> userRepositoryQuery, IUserService userService
            , INotificationRepository notificationRepository, IAuditRepo AuditRepo, IRepositoryQuery<OptometristFirm> optometristQuery, IRepositoryQuery<OptometristFirmUser> optometristUserQuery, IUserRepository userRepository, UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<OptometristManagementController> logger)
        {
            _userRepository = userRepository;
            _optometristQuery = optometristQuery;
            _optometristUserQuery = optometristUserQuery;
            _userRepositoryQuery = userRepositoryQuery;
            _notificationRepository = notificationRepository;
            _AuditRepo = AuditRepo;
            currentUserId = userService.GetUserData().Id;
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }
        // GET: User
        public async Task<IActionResult> Index()
        {
            var optometristFirmUser = _optometristUserQuery.FilterAsync(x => x.ApplicationUserId == currentUserId).Result.FirstOrDefault();
            int OptometristFirmId = optometristFirmUser == null ? 0 : optometristFirmUser.OptometristFirmId;
            ApplicationUser user = await _userManager.FindByIdAsync(currentUserId);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            var users = _userRepository.GetUsers(AppRoles.SYSTEMADMIN, currentUserId).Where(x => x.OptometristFirmId == OptometristFirmId && roles.Contains(AppRoles.OPTOMETRIST));
            _AuditRepo.AddAudit(Activities.VIEW_OPTOMETRIST, "View Optomstrist");
            return View(users);
        }


        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Customer", new { area = "Customer" });
            }
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            var model = new UserModel
            {
                OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId,
                RoleName = AppRoles.OPTOMETRIST
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            try
            {
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

                using (var trans = new TransactionScope())
                {
                    var applicationUser = new ApplicationUser()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatedDate = DateTime.Now,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        IsActive = true,
                        MobileNumber = model.MobileNumber,
                        UserName = model.Email,
                        CreatedBy = currentUserId,
                        DefaultRole = AppRoles.OPTOMETRIST,
                        OptometristFirmId = model.OptometristFirmId,
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
                        await _userManager.AddToRoleAsync(applicationUser, AppRoles.OPTOMETRIST);

                        var code = _userManager.GeneratePasswordResetTokenAsync(applicationUser);

                        var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = userId, code = code });
                        string mPre = $"{_configuration["AppConstants:BaseUrl"]}/{callbackUrl}";

                        _notificationRepository.SendNewAccountCreated(applicationUser, pwd, mPre);
                    }
                    else
                    {
                        
                        model.Errors.Add("Unable to save record");
                        return View(model);
                    }
                    trans.Complete();
                }

                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.CREATE_OPTOMETRIST, "Create Optometrist");
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
            var model = _userRepository.GetUserDetails(Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(UserViewModel model, string Id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }

            try
            {

                string administratorId = currentUserId;
                model.OptometristFirmId = _optometristUserQuery.FilterAsync(x => x.ApplicationUserId == administratorId).Result.FirstOrDefault().OptometristFirmId;

                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                string responseMessage = "";
                bool result = _userRepository.Update(model, currentUserId, out responseMessage);
                if (result)
                {
                    TempData["SuccessMessage"] =  responseMessage;
                    _AuditRepo.AddAudit(Activities.UPDATE_OPTOMETRIST, "Update Optometrist");
                    return RedirectToAction("Index");
                }
                else
                {
                    model.Errors.Add(responseMessage);
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

                //string mDesc = $"Password for User {user.FullName} has been reset successfully";
                _AuditRepo.AddAudit(Activities.RESET_PASSWORD, "Reset Password For Optometrist");
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
                }
                else
                {
                    message = "Password for User " + applicationUser.FullName + " has been activated successfully";
                    applicationUser.IsActive = true;
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