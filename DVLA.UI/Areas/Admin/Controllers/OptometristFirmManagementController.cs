using DVLA.Business.NotificationModule;
using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "System Administrator")]
    public class OptometristFirmManagementController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IRepositoryQuery<Region> _regionQuery;
        private readonly IRepositoryQuery<District> _districtQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IReportRepository _reportRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<OptometristFirmManagementController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly DVLADbContext _context;
        private readonly IAuthUser _authUser;
        private readonly static object _locker = new object();

        // GET: Admin/OptometristManagement
        public OptometristFirmManagementController(IAuditRepo AuditRepo, IRepositoryQuery<OptometristFirm> optometristQuery, IRepositoryQuery<Region> regionQuery,
            IRepositoryQuery<District> districtQuery,
            INotificationRepository notificationRepository, IUserService userService, IRepositoryQuery<OptometristFirmUser> optometristUserQuery, IReportRepository reportRepository, IUserRepository userRepository, ILogger<OptometristFirmManagementController> logger, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration, DVLADbContext context, IAuthUser authUser)
        {
            _AuditRepo = AuditRepo;
            _regionQuery = regionQuery;
            _optometristQuery = optometristQuery;
            _optometristUserQuery = optometristUserQuery;
            _notificationRepository = notificationRepository;
            _districtQuery = districtQuery;
            _reportRepository = reportRepository;
            _userRepository = userRepository;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _authUser = authUser;
        }

        [HttpGet]
        // GET: Admin/Optometrist
        public async Task<ActionResult> Index()
        {
            try
            {
                ViewBag.Region = 0;
                if (HttpContext.Session.GetString("Regions") == null)
                {
                    HttpContext.Session.SetString("Regions", JsonConvert.SerializeObject(_regionQuery.GetAllAsync().GetAwaiter().GetResult()));
                }
                ViewBag.Regions = JsonConvert.DeserializeObject<List<Region>>(HttpContext.Session.GetString("Regions"));
                var model = await _reportRepository.FetchAllOptometristFirms(0, null);
                model = model.Where(x => !x.IsActive).ToList();
                _AuditRepo.AddAudit(Activities.VIEW_OPTOMETRIST_FIRM, "View Optometrist Firm");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(new List<OptometristFirmModel>());

        }


        [HttpPost]
        public async Task<JsonResult> GetOptometristFirms(int? Region, int? District) 
        {
            var model = await _reportRepository.FetchAllOptometristFirms(Region, District);
            return Json(model);
        }


        [HttpPost]
        // GET: Admin/Optometrist
        public async Task<ActionResult> Index(int? Region, int? District)
        {
            try
            {
                ViewBag.Region = Region;
                ViewBag.District = District;
                if (HttpContext.Session.GetString("Regions") == null)
                {
                    HttpContext.Session.SetString("Regions", JsonConvert.SerializeObject(_regionQuery.GetAllAsync().GetAwaiter().GetResult().OrderBy(x => x.Name)));
                }
                ViewBag.Regions = JsonConvert.DeserializeObject<List<Region>>(HttpContext.Session.GetString("Regions"));
                var model = await _reportRepository.FetchAllOptometristFirms(Region, District);
                model = model.Where(x => x.IsActive).ToList();
                HttpContext.Session.SetString("ExportItems", JsonConvert.SerializeObject(model));
                _AuditRepo.AddAudit(Activities.VIEW_OPTOMETRIST_FIRM, "View Optometrist Firm");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(new List<OptometristFirmModel>());

        }


        [HttpGet]
        public ActionResult ExportOptometristFirms()
        {
            try
            {
                if (HttpContext.Session.GetString("ExportItems") == null)
                {
                    return NoContent();
                }


                List<OptometristFirmModel> model = JsonConvert.DeserializeObject<List<OptometristFirmModel>>(HttpContext.Session.GetString("ExportItems"));
               
                //Convert List to Excel
                List<OptometristFirmExcelExport> exportItems = model.Select(x => new OptometristFirmExcelExport
                {
                    AccreditationNumber = x.AccreditationNumber,
                    BusinessAddress = x.BusinessAddress,
                    BusinessName = x.BusinessName,
                    CentreCode = x.CentreCode,
                    ContactEmailAddress = x.ContactEmailAddress,
                    ContactFirstName = x.ContactFirstName,
                    ContactLastName = x.ContactLastName,
                    ContactPhoneNumber = x.ContactLastName,
                    DigitalAddress = x.DigitalAddress,
                    DistrictName = x.DistrictName,
                    MobileNumber = x.MobileNumber,
                    RegionName = x.RegionName,
                    RegistrationNumber = x.RegistrationNumber,
                    ReorderLevel = x.ReorderLevel,
                    TelephoneNumber = x.TelephoneNumber,
                    Town = x.Town
                }).ToList();
                byte[] exportData = Utility.ExportToExcel(exportItems);
                _AuditRepo.AddAudit(Activities.VIEW_OPTOMETRIST_FIRM, "View Optometrist Firm");

                return File(exportData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OptometristFirms.xlsx");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(new List<OptometristFirmModel>());
        }

        public ActionResult Deactivated()
        {
            try
            {
                var obj = _optometristQuery.Filter(x => !x.IsActive).Join(_regionQuery.GetAllInclude(x => x.Districts),
               o => o.RegionId, r => r.Id, (o, r) => new { o, r })
               .Select(p => new OptometristFirmModel
               {
                   AccreditationNumber = p.o.AccreditationNumber,
                   BusinessAddress = p.o.BusinessAddress,
                   BusinessName = p.o.BusinessName,
                   CentreCode = p.o.CentreCode,
                   ContactEmailAddress = p.o.ContactEmail,
                   ContactFirstName = p.o.ContactFirstName,
                   ContactLastName = p.o.ContactLastName,
                   ContactPhoneNumber = p.o.ContactPhoneNumber,
                   CreatedBy = p.o.CreatedBy,
                   DigitalAddress = p.o.DigitalAddress,
                   Id = p.o.Id,
                   IsActive = p.o.IsActive,
                   IsDeleted = p.o.IsDeleted,
                   MobileNumber = p.o.MobileNumber,
                   RegionId = p.o.RegionId,
                   DistrictId = p.o.DistrictId,
                   DistrictName = p.r.Districts.FirstOrDefault(x => x.Id == p.o.DistrictId).Name,
                   RegionName = p.r.Name,
                   RegistrationNumber = p.o.RegistrationNumber,
                   ReorderLevel = p.o.ReorderLevel,
                   TelephoneNumber = p.o.TelephoneNumber,
                   Town = p.o.Town,
                   UpdatedBy = p.o.CreatedBy
               }).ToList();


                //var optometrist = (from x in _optometristQuery.GetAllList()
                //                   join y in _regionQuery.GetAllList()
                //                   on x.RegionId equals y.Id
                //                   // where x.id.Equals(id)
                //                   select new OptometristFirmModel
                //                   {
                //                       Id = x.Id,
                //                       AccreditationNumber = x.AccreditationNumber,
                //                       BusinessAddress = x.BusinessAddress,
                //                       BusinessName = x.BusinessName,
                //                       CentreCode = x.CentreCode,
                //                       RegionName = y.Name,
                //                       DigitalAddress = x.DigitalAddress,
                //                       RegistrationNumber = x.RegistrationNumber,
                //                       ContactEmailAddress = x.ContactEmail,
                //                       ContactFirstName = x.ContactFirstName,
                //                       ContactLastName = x.ContactLastName,
                //                       ContactPhoneNumber = x.ContactPhoneNumber,
                //                       MobileNumber = x.MobileNumber,
                //                       TelephoneNumber = x.TelephoneNumber,
                //                       Town = x.Town,
                //                       IsActive = x.IsActive,
                //                       CreatedBy = x.CreatedBy,
                //                       IsDeleted = x.IsDeleted,
                //                       //RegionCode = x.RegionCode,
                //                       UpdatedBy = x.UpdatedBy

                //                   }).ToList();
                _AuditRepo.AddAudit(Activities.VIEW_OPTOMETRIST_FIRM, "View Optometrist Firm");
                return View(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(new List<OptometristFirmModel>());
        }

        public ActionResult Create()
        {
            OptometristFirmModel model = new();
            model.Regions = _regionQuery.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            return View(model);
        }

        [HttpPost]

        public ActionResult Create(OptometristFirmModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                model.Regions = _regionQuery.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
                model.Districts = _districtQuery.GetAll().Where(p => p.RegionId == model.RegionId).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();

                if (!ModelState.IsValid)
                {
                    model.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return View(model);
                }

                var businessName = _optometristQuery.FilterAsync(x => x.BusinessName == model.BusinessName).Result.FirstOrDefault();
                if (businessName != null)
                {
                    model.Errors.Add("Business name already exist");
                    return View(model);
                }

                var registrationNumber = _optometristQuery.FilterAsync(x => x.RegistrationNumber == model.RegistrationNumber).Result.FirstOrDefault();
                if (registrationNumber != null)
                {
                    model.Errors.Add("Registration number already exist");
                    return View(model);
                }

                //var accreditationNumber = _optometristQuery.FilterAsync(x => x.AccreditationNumber == model.AccreditationNumber).Result.FirstOrDefault();
                //if (accreditationNumber != null)
                //{
                //    model.Errors.Add("Accreditation number already exist");
                //    return View(model);
                //}

                lock (_locker)
                {
                    var context = _context;
                    var scope = context.Database.BeginTransaction();
                    using (scope)
                    {
                        var user = _userManager.FindByEmailAsync(model.ContactEmailAddress).GetAwaiter().GetResult();
                        if (user != null)
                        {
                            scope.Rollback();
                            model.Errors.Add("User already exist");
                            return View(model);
                        }

                        var applicationUser = new ApplicationUser()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = model.ContactEmailAddress,
                            FirstName = model.ContactFirstName,
                            LastName = model.ContactLastName,
                            IsActive = true,
                            MobileNumber = model.MobileNumber,
                            Address = model.BusinessAddress,
                            UserName = model.ContactEmailAddress,
                            CreatedBy = _authUser.UserId,
                            CreatedDate = DateTime.UtcNow,
                            DefaultRole = AppRoles.FACILITYOWNER,
                            EmailConfirmed = true,
                            PhoneNumber = model.ContactPhoneNumber
                        };

                        var pwd = Guid.NewGuid().ToString().Replace("-", "").Substring(1, 6);
                        applicationUser.IsActive = true;

                        var admin = _userManager.CreateAsync(applicationUser, pwd).GetAwaiter().GetResult();

                        if (admin.Succeeded)
                        {
                            //OPT/DVLA/0000/24(CURRENT YEAR)
                            string currentYear = DateTime.Now.ToString("yy");
                            OptometristFirm optomestristFirm = context.OptometristFirms.OrderByDescending(x => x.Id).FirstOrDefault();
                            if (optomestristFirm == null)
                            {
                                model.AccreditationNumber = $"DVLA/0000/{DateTime.Now.ToString("yy")}";
                            }
                            else
                            {
                                int tokenLength = optomestristFirm.AccreditationNumber.Split('/').Length;
                                if (tokenLength != 4)
                                {
                                    model.AccreditationNumber = $"OPT/DVLA/0000/{DateTime.Now.ToString("yy")}";
                                }
                                else
                                {

                                    string incrementString = optomestristFirm.AccreditationNumber.Split('/')[2];
                                    string year = optomestristFirm.AccreditationNumber.Split('/')[3];
                                    int incrementer;
                                    if (currentYear == year)
                                    {

                                        if (int.TryParse(incrementString, out incrementer))
                                        {
                                            incrementer = incrementer + 1;
                                        }
                                        else
                                        {
                                            incrementer = 0;
                                        }
                                        incrementString = incrementer.ToString().PadLeft(4, '0');
                                        model.AccreditationNumber = $"OPT/DVLA/{incrementString}/{currentYear}";
                                    }
                                    else
                                    {
                                        incrementer = 0;
                                        incrementString = incrementer.ToString().PadLeft(4, '0');
                                        model.AccreditationNumber = $"OPT/DVLA/{incrementString}/{currentYear}";
                                    }
                                }
                            }


                            optomestristFirm = new OptometristFirm
                            {
                                RegionId = model.RegionId,
                                DistrictId = model.DistrictId,
                                AccreditationNumber = model.AccreditationNumber,
                                BusinessAddress = model.BusinessAddress,
                                BusinessName = model.BusinessName,
                                ContactEmail = model.ContactEmailAddress,
                                ContactFirstName = model.ContactFirstName,
                                ContactLastName = model.ContactLastName,
                                ContactPhoneNumber = model.ContactPhoneNumber,
                                CreatedBy = _authUser.UserId,
                                DigitalAddress = model.DigitalAddress,
                                IsActive = true,
                                IsDeleted = false,
                                MobileNumber = model.MobileNumber,
                                RegistrationNumber = model.RegistrationNumber,
                                Town = model.Town,
                                TelephoneNumber = model.TelephoneNumber
                            };
                            _optometristQuery.Add(optomestristFirm);

                            //applicationUser.OptometristFirmId = optomestristFirm.Id;
                            _userManager.UpdateAsync(applicationUser).GetAwaiter().GetResult();
                            //_optometristCommand.SaveChanges();
                            //Generate Reference Number
                            string regionPrefix = _regionQuery.Filter(x => x.Id == model.RegionId).FirstOrDefault().PrefixName;
                            optomestristFirm.CentreCode = "VA" + DateTime.Now.ToString("yyyymmdd").Substring(2, 2) + optomestristFirm.Id.ToString().PadLeft(6, '0') + regionPrefix;
                            _optometristQuery.UpdateAsync(optomestristFirm).GetAwaiter().GetResult();

                            _optometristUserQuery.Add(new OptometristFirmUser()
                            {
                                OptometristFirmId = optomestristFirm.Id,
                                ApplicationUserId = applicationUser.Id
                            });
                            //_optometristUserCommand.SaveChanges();

                            var userId = applicationUser.Id;
                            _userManager.AddToRoleAsync(applicationUser, AppRoles.FACILITYOWNER).GetAwaiter().GetResult();

                            var code = _userManager.GeneratePasswordResetTokenAsync(applicationUser).GetAwaiter().GetResult();

                            var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = userId, code = code });
                            string mPre = $"{_configuration["AppConstants:BaseUrl"]}{callbackUrl}";

                            _notificationRepository.SendNewAccountCreated(applicationUser, pwd, mPre, context);
                            scope.Commit();
                        }
                        else
                        {
                            scope.Rollback();
                            model.Errors.Add("Unable to save record, user already exist");
                            return View(model);
                        }
                    }
                }



                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.CREATE_OPTOMETRIST_FIRM, "Create Optometrist Firm");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "Admin", new { area = "Admin" });
                }
                var model = new OptometristFirmModel();
                model.Regions = _regionQuery.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();


                // Convert byte array back to original string

                int optometristId = id;//Convert.ToInt32(Utility.Decrypt(token));
                OptometristFirm optometrist = _optometristQuery.Filter(x => x.Id == optometristId).FirstOrDefault();
                var optometristUser = _optometristUserQuery.Filter(u => u.OptometristFirmId == optometristId).FirstOrDefault();
                var region = _regionQuery.Filter(r => r.Id == optometrist.RegionId).FirstOrDefault();
                var district = _districtQuery.Filter(r => r.Id == optometrist.DistrictId).FirstOrDefault();

                model.Districts = _districtQuery.GetAll().Where(p => p.RegionId == optometrist.RegionId).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();



                model.Id = optometrist.Id;
                model.RegionId = optometrist.RegionId;
                model.DistrictId = optometrist.DistrictId;
                model.UserId = optometristUser.ApplicationUserId;
                model.AccreditationNumber = optometrist.AccreditationNumber;
                model.BusinessAddress = optometrist.BusinessAddress;
                model.BusinessName = optometrist.BusinessName;
                model.RegionName = region.Name;
                model.DigitalAddress = optometrist.DigitalAddress;
                model.RegistrationNumber = optometrist.RegistrationNumber;
                model.ContactEmailAddress = optometrist.ContactEmail;
                model.ContactFirstName = optometrist.ContactFirstName;
                model.ContactLastName = optometrist.ContactLastName;
                model.ContactPhoneNumber = optometrist.ContactPhoneNumber;
                model.MobileNumber = optometrist.MobileNumber;
                model.TelephoneNumber = optometrist.TelephoneNumber;
                model.Town = optometrist.Town;
                model.IsActive = optometrist.IsActive;
                model.CreatedBy = optometrist.CreatedBy;
                model.IsDeleted = optometrist.IsDeleted;
                model.UpdatedBy = optometrist.ModifiedBy;


                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return View(new OptometristFirmModel());
        }

        [HttpPost]
        public ActionResult Update(OptometristFirmModel model)
        {
            model.Regions = _regionQuery.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            model.Districts = _districtQuery.GetAll().Where(p => p.RegionId == model.RegionId).Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
            try
            {
                //model.Id = Convert.ToInt32(Utility.Decrypt(Id));

                if (!ModelState.IsValid)
                {
                    model.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return View(model);
                }

                var optomestrist = _optometristQuery.Filter(x => x.Id == model.Id).FirstOrDefault();
                string oldContactEmail = optomestrist.ContactEmail;
                if (optomestrist == null)
                {
                    model.Errors.Add("No optometrist record found");
                    return View(model);
                }
                if (optomestrist.ContactEmail != model.ContactEmailAddress)
                {
                    model.Errors.Add("You cannot update Email. Please create another account");
                    return View(model);
                }

                optomestrist.RegionId = model.RegionId;
                optomestrist.DistrictId = model.DistrictId;
                //optomestrist.AccreditationNumber = model.AccreditationNumber;
                optomestrist.BusinessAddress = model.BusinessAddress;
                optomestrist.BusinessName = model.BusinessName;
                optomestrist.ContactEmail = model.ContactEmailAddress;
                optomestrist.ContactFirstName = model.ContactFirstName;
                optomestrist.ContactLastName = model.ContactLastName;
                optomestrist.ContactPhoneNumber = model.ContactPhoneNumber;
                optomestrist.CreatedBy = _authUser.UserId;
                optomestrist.DigitalAddress = model.DigitalAddress;
                optomestrist.IsActive = true;
                optomestrist.IsDeleted = false;
                optomestrist.MobileNumber = model.MobileNumber;
                //optomestrist.RegistrationNumber = model.RegistrationNumber;
                optomestrist.Town = model.Town;
                optomestrist.TelephoneNumber = model.TelephoneNumber;

                _optometristQuery.Update(optomestrist);

                //if (!oldContactEmail.Equals(model.ContactEmailAddress))
                //{
                //    var applicationUser = new ApplicationUser()
                //    {
                //        Email = model.ContactEmailAddress,
                //        FirstName = model.ContactFirstName,
                //        LastName = model.ContactLastName,
                //        IsActive = true,
                //        MobileNumber = model.ContactPhoneNumber,
                //        Address = model.BusinessAddress,
                //        UserName = model.ContactEmailAddress,
                //        CreatedBy = currentUserId
                //    };

                //    var pwd = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 6);
                //    applicationUser.IsActive = true;

                //    var admin = await _userManager.CreateAsync(applicationUser, pwd);
                //    if (admin.Succeeded)
                //    {
                //        _optometristUserQuery.Add(new OptometristFirmUser()
                //        {
                //            OptometristFirmId = optomestrist.Id,
                //            ApplicationUserId = applicationUser.Id
                //        });

                //        var userId = applicationUser.Id;
                //        await _userManager.AddToRoleAsync(applicationUser, AppRoles.FACILITYOWNER);

                //        var code = await _userManager.GeneratePasswordResetTokenAsync(applicationUser);

                //        var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "", userId = userId, code = code });
                //        string mPre = _configuration["AppConstants:BaseUrl"] + callbackUrl;

                //        _notificationRepository.SendNewAccountCreated(applicationUser, pwd, mPre);
                //    }

                //}


                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.UPDATE_OPTOMETRIST_FIRM, "Update otpmetrist Firm");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }


        public JsonResult GetDetails(int Id)
        {
            try
            {
                var optometrist = _optometristQuery.Filter(x => x.Id == Id).FirstOrDefault();
                var optometristUser = _optometristUserQuery.Filter(u => u.OptometristFirmId == Id).FirstOrDefault();
                var region = _regionQuery.Filter(r => r.Id == optometrist.RegionId).FirstOrDefault();
                var district = _districtQuery.Filter(r => r.Id == optometrist.DistrictId).FirstOrDefault();
                var result = new OptometristFirmModel
                {
                    Id = optometrist.Id,
                    UserId = optometristUser.ApplicationUserId,
                    AccreditationNumber = optometrist.AccreditationNumber,
                    BusinessAddress = optometrist.BusinessAddress,
                    BusinessName = optometrist.BusinessName,
                    RegionName = region.Name,
                    DistrictName = district.Name,
                    DigitalAddress = optometrist.DigitalAddress,
                    RegistrationNumber = optometrist.RegistrationNumber,
                    ContactEmailAddress = optometrist.ContactEmail,
                    ContactFirstName = optometrist.ContactFirstName,
                    ContactLastName = optometrist.ContactLastName,
                    ContactPhoneNumber = optometrist.ContactPhoneNumber,
                    MobileNumber = optometrist.MobileNumber,
                    TelephoneNumber = optometrist.TelephoneNumber,
                    Town = optometrist.Town,
                    IsActive = optometrist.IsActive,
                    CreatedBy = optometrist.CreatedBy,
                    IsDeleted = optometrist.IsDeleted,
                    UpdatedBy = optometrist.ModifiedBy,

                };

                return Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return Json(new { success = false, message = "Kindly try again later" });
            }
        }


        public async Task<JsonResult> ChangeStatus(int Id)
        {
            string message = "";
            var scope = await _context.Database.BeginTransactionAsync();
            using (scope)
            {
                try
                {
                    var optometrist = _optometristQuery.Filter(x => x.Id == Id).FirstOrDefault();
                    if (optometrist == null)
                    {
                       await scope.RollbackAsync();
                        message = "Optometrist Firm does not exist";
                        return Json(new { success = false, message });
                    }
                    optometrist.IsActive = !optometrist.IsActive;
                    optometrist.ModifiedBy = _authUser.UserId;
                    optometrist.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    string respMessage = "";
                    var optometristUsers = _context.OptometristFirmUsers.Where(x => x.OptometristFirmId == optometrist.Id);
                    var userIds = optometristUsers.Select(x => x.ApplicationUserId);
                    foreach (var user in userIds)
                    {

                        var applicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == user);
                        if (applicationUser != null)
                        {
                            applicationUser.IsActive = optometrist.IsActive;
                            _context.SaveChanges();
                        }
                    }

                    await scope.CommitAsync();
                    return Json(new { success = true, message });
                }
                catch (Exception ex)
                {
                    await scope.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                    return Json(new { success = false, message = "Kindly try again later" });
                }
            }
            
        }

        [HttpGet]
        public async Task<ActionResult> ChangeOptometristFirmStatus(int Id)
        {
            string message = "";
            var scope = await _context.Database.BeginTransactionAsync();
            using (scope)
            {
                try
                {
                    var optometrist = await _context.OptometristFirms.FirstOrDefaultAsync(x => x.Id == Id);
                    if (optometrist == null)
                    {
                        await scope.RollbackAsync();
                        TempData["ErrorMessage"] = "Optometrist Firm does not exist";
                        return RedirectToAction("Index");
                    }
                    optometrist.IsActive = !optometrist.IsActive;
                    optometrist.ModifiedBy = _authUser.UserId;
                    optometrist.ModifiedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    string respMessage = "";
                    var optometristUsers = _context.OptometristFirmUsers.Where(x => x.OptometristFirmId == optometrist.Id);
                    var userIds = optometristUsers.Select(x => x.ApplicationUserId);
                    foreach (var user in userIds)
                    {

                        var applicationUser = _context.ApplicationUsers.FirstOrDefault(x => x.Id == user);
                        if (applicationUser != null)
                        {
                            applicationUser.IsActive = optometrist.IsActive;
                            _context.SaveChanges();
                        }
                    }

                    await scope.CommitAsync();
                    TempData["SuccessMessage"] = "Changes successful";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    await scope.RollbackAsync();
                    _logger.LogError(ex.Message, ex);
                    TempData["ErrorMessage"] = "Error occurred. Try again later" + ex.Message + ex.StackTrace;
                    return RedirectToAction("Index");
                }
            }

        }

        public JsonResult GetDistrict(int id)
        {
            List<SelectListItem> districtNames = new List<SelectListItem>();
            try
            {


                if (id != 0)
                {

                    var districts = _districtQuery.GetAllAsync().Result.Where(x => x.RegionId == id).ToList();
                    districts.ForEach(x =>
                    {
                        districtNames.Add(new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
                    });
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return Json(districtNames);

        }

        [HttpPost]
        public ActionResult ExportFirm(List<OptometristFirmModel> model)
        {
            if (model != null)
            {
                var list = model.Select(x => new
                {
                    BusinessName = x.BusinessName,
                    BusinessRegistrationNumber = x.RegistrationNumber,
                    DigitalAddress = x.DigitalAddress,
                    BusinessTelephoneNumber = x.TelephoneNumber,
                    BusinessAddress = x.BusinessAddress,
                    Town = x.Town,
                    DVLAAccreditationNumber = x.AccreditationNumber,
                    Region = x.RegionName,
                    District = x.DistrictName,
                    ContactPersonLastName = x.ContactLastName,
                    ContactPersonOtherName = x.ContactFirstName,
                    ContactPersonPhoneNumber = x.ContactPhoneNumber,
                    ContactPersonEmailAddress = x.ContactEmailAddress,
                    CentreCode = x.CentreCode,
                    ReorderLevel = x.ReorderLevel
                }).ToList();
                string fileName = "OptometristFirms_Report.xlsx";
                var json = JsonConvert.SerializeObject(list);
                byte[] report = _reportRepository.WriteToExcel("xlsx", (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable))));
                return File(report, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

    }
}