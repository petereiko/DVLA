
using DVLA.Business.LocationModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace DVLA.UI.Areas.Registration.Controllers
{
    [Authorize(Roles = AppRoles.FRONTOFFICER)]

    [Area("Registration")]

    public class FrontOfficeController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IRepositoryQuery<VisualAssessmentResult> _applicantQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristFirmUserQuery;
        private readonly IReportRepository _reportRepository;
        private readonly ISmsRepository _smsRepository;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly ILogger<FrontOfficeController> _logger;
        private readonly string currentUserId;
        private readonly IWebHostEnvironment _environment;
        private readonly ILocationService _locationService;

        // GET: FrontOffice/Applicant
        public FrontOfficeController(IAuditRepo AuditRepo, IRepositoryQuery<VisualAssessmentResult> applicantQuery, IRepositoryQuery<Region> regionQuery,
            INotificationRepository notificationRepository, IUserService userService, IReportRepository reportRepository, ISmsRepository smsRepository, IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<OptometristFirm> optometristFirmQuery, IAuthUser authUser, ILogger<FrontOfficeController> logger, IWebHostEnvironment environment, IRepositoryQuery<OptometristFirmUser> optometristFirmUserQuery, ILocationService locationService)
        {
            _AuditRepo = AuditRepo;
            _applicantQuery = applicantQuery;
            _notificationRepository = notificationRepository;
            _reportRepository = reportRepository;
            _smsRepository = smsRepository;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _optometristFirmQuery = optometristFirmQuery;
            _logger = logger;
            currentUserId = authUser.UserId;
            _environment = environment;
            _optometristFirmUserQuery = optometristFirmUserQuery;
            _locationService = locationService;
        }

        // GET: Admin/Optometrist
        public ActionResult Index()
        {
            try
            {
                var optometristUser = _optometristFirmUserQuery.Filter(u => u.ApplicationUserId == currentUserId).FirstOrDefault();
                var query = _applicantQuery.Filter(x => x.IsRegistration == true && x.Status == Status.InProgress && x.OptometristFirmId == optometristUser.OptometristFirmId);
                //await _smsRepository.SendPendingSms();
                
                var obj = _applicantQuery.GetAll().Join(_optometristFirmQuery.GetAll(),
                    a => a.OptometristFirmId,
                    o => o.Id, (a, o) => new { a, o }).Where(x => x.a.IsRegistration == true && x.a.OptometristFirmId == optometristUser.OptometristFirmId && x.a.Status==Status.InProgress)
                    .Select(p => new ApplicantModel
                    {
                        ContactNumber = p.a.ContactNumber,
                        FirstName = p.o.BusinessAddress,
                        DOB = p.a.DOB,
                        Status = p.a.Status,
                        Email = p.a.Email,
                        //NameTitle = p.a.NameTitle,
                        OtherName = p.a.OtherName,
                        Fullname = p.a.Surname + " " + p.a.FirstName + " " + p.a.OtherName,
                        CreatedBy = p.o.CreatedBy,
                        Optometrist = p.o.BusinessName,
                        OptometristFirmId = p.o.Id,
                        PostalAddress = p.a.PostalAddress,
                        Surname = p.a.Surname,
                        Nationality = p.a.Nationality,
                        DateCreated = p.a.CreatedDate,
                        Id = p.a.Id,
                        IsActive = p.o.IsActive,
                        IsDeleted = p.o.IsDeleted,
                        UpdatedBy = p.o.ModifiedBy
                    }).OrderByDescending(a => a.Id).ToList();
                    _AuditRepo.AddAudit(Activities.APPLICANT_REGISTRATION, "View Applicant List");
                return View(obj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(new List<ApplicantModel>());
        }

        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "FrontOffice", new { area = "Registration" });
            }
            var countries = _locationService.GetCountries();

            ViewBag.Countries = countries;
            var optometristUser = _optometristFirmUserQuery.FilterInclude(u => u.ApplicationUserId == currentUserId, x=>x.OptometristFirm).FirstOrDefault();
            _optometristFirmQuery.Filter(o => o.Id == optometristUser.OptometristFirmId);

            return View(new ApplicantModel { OptometristFirmId = optometristUser.OptometristFirmId, Optometrist = optometristUser.OptometristFirm.BusinessName });
        }

        [HttpPost]
        public async Task<ActionResult> Create(ApplicantModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "FrontOffice", new { area = "Registration" });
                }
                //ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();

                var countries = _locationService.GetCountries();

                ViewBag.Countries = countries;

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
                
                

                if (model.ResultServiceType == null)
                {
                    ModelState.AddModelError("ResultServiceType", "Please select service type");
                }

                if (string.IsNullOrEmpty(model.Surname))
                {
                    ModelState.AddModelError("Surname", "Please enter surname");
                }

                if (string.IsNullOrEmpty(model.FirstName))
                {
                    ModelState.AddModelError("FirstName", "Please enter first name");
                }

                if (model.DOB == null)
                {
                    ModelState.AddModelError("DOB", "Please select DOB");
                }

                if (string.IsNullOrEmpty(model.PostalAddress))
                {
                    ModelState.AddModelError("PostalAddress", "Please enter postal address");
                }

                if (model.OptometristFirmId == 0)
                {
                    ModelState.AddModelError("OptometristFirmId", "Please select Optometrist Firm");
                }

                if (string.IsNullOrEmpty(model.ContactNumber))
                {
                    ModelState.AddModelError("ContactNumber", "Please enter contact number");
                }

                //if (string.IsNullOrEmpty(model.TaxIdentificationNumber))
                //{
                //    ModelState.AddModelError("TaxIdentificationNumber", "Please enter tax identification number");
                //}

                if (!ModelState.IsValid)
                {
                    model.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    return View(model);
                }

                //string referenceNumber = _visualAssessmentResultRepository.GenerateFormNo();

                var applicant = new VisualAssessmentResult()
                {
                    CreatedDate = DateTime.Now,
                    Id = model.Id,
                    Surname = model.Surname,
                    OptometristFirmId = model.OptometristFirmId,
                    ResultServiceType = model.ResultServiceType,
                    AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory,
                    PassportImageUrl = model.Filename,
                    TestType = model.TestType,
                    Status = Status.InProgress,
                    FirstName = model.FirstName,
                    OtherName = model.OtherName,
                    DOB = (DateTime)model.DOB,
                    PostalAddress = model.PostalAddress,
                    ContactNumber = model.ContactNumber,
                    Nationality = model.Nationality,
                    Email = model.Email,
                    IsRegistration = true,
                    CreatedBy = currentUserId,
                    Gender = model.Gender
                };

                await _applicantQuery.AddAsync(applicant);



                //await _smsRepository.SendRegistrationDetail(model.FirstName, model.ContactNumber, referenceNumber);
                //send email
               // await _notificationRepository.SendRegistrationDetail(model.FirstName, model.ContactNumber, referenceNumber, model.Email);



                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Visual Assessment Registration");

                
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
        public ActionResult Update(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "FrontOffice", new { area = "Registration" });
                }
                ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();

                var countries = _locationService.GetCountries();

                ViewBag.Countries = countries;

                Int64 applicanId =Convert.ToInt64(Utility.Decrypt(token));
                var applicant = _applicantQuery.Filter(x => x.Id == applicanId).FirstOrDefault();
                var optometristUser = _optometristFirmUserQuery.Filter(u => u.ApplicationUserId == currentUserId).FirstOrDefault();
                var firm = _optometristFirmQuery.Filter(x => x.Id == optometristUser.OptometristFirmId).FirstOrDefault();

                var model = new ApplicantModel();
                model.Id = applicant.Id;
                model.Surname = applicant.Surname;
                //model.DriversLicence = applicant.DriversLicence;
                //model.DVLAReferenceNo = applicant.DVLAReferenceNo;
                model.FirstName = applicant.FirstName;
                model.OtherName = applicant.OtherName;
                model.DOB = (DateTime)applicant.DOB;
                model.PostalAddress = applicant.PostalAddress;
                model.ContactNumber = applicant.ContactNumber;
                model.Nationality = applicant.Nationality;
                model.Email = applicant.Email;
                model.ResultServiceType = applicant.ResultServiceType;
                //model.PassportImageUrl = applicant.PassportImageUrl;
                model.Gender = applicant.Gender;
                model.Status = applicant.Status;
                model.OptometristFirmId = applicant.OptometristFirmId;
                model.Optometrist = firm.BusinessName;
                //model.FormNumber = applicant.FormNumber;
                model.TestType = (TestType)applicant.TestType;
                //model.InvoiceNumber = applicant.OldDVLAReferenceNo;
                model.IsActive = applicant.IsActive;
                model.CreatedBy = applicant.CreatedBy;
                model.IsDeleted = applicant.IsDeleted;
                model.UpdatedBy = applicant.ModifiedBy;
                model.PassportImageUrl = applicant.PassportImageUrl;

                if (!string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    var path = Path.Combine(_environment.WebRootPath, "Passports", model.PassportImageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        byte[] imageArray = System.IO.File.ReadAllBytes(path);
                        model.PassportImageUrl = Convert.ToBase64String(imageArray);
                    }

                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return View(new ApplicantModel());
        }
             
        [HttpPost]
        public ActionResult Update(ApplicantModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "FrontOffice", new { area = "Registration" });
            }
            ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();

            var countries = _locationService.GetCountries();

            ViewBag.Countries = countries;
            try
            {
                if (model.ResultServiceType == null)
                {
                    ModelState.AddModelError("ResultServiceType", "Please select service type");
                }
                if (string.IsNullOrEmpty(model.Surname))
                {
                    ModelState.AddModelError("Surname", "Please enter surname");
                }

                if (string.IsNullOrEmpty(model.FirstName))
                {
                    ModelState.AddModelError("FirstName", "Please enter first name");
                }

                if (model.DOB == null)
                {
                    ModelState.AddModelError("DOB", "Please select DOB");
                }

                if (string.IsNullOrEmpty(model.PostalAddress))
                {
                    ModelState.AddModelError("PostalAddress", "Please enter postal address");
                }

                if (model.OptometristFirmId == 0)
                {
                    ModelState.AddModelError("OptometristFirmId", "Please select Optometrist Firm");
                }

                if (string.IsNullOrEmpty(model.ContactNumber))
                {
                    ModelState.AddModelError("ContactNumber", "Please enter contact number");
                }

                //if (string.IsNullOrEmpty(model.TaxIdentificationNumber))
                //{
                //    ModelState.AddModelError("TaxIdentificationNumber", "Please enter tax identification number");
                //}

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var applicant = _applicantQuery.Filter(x => x.Id == model.Id).FirstOrDefault();

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
                //model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);




                //applicant.NameTitle = model.NameTitle;
                applicant.Surname = model.Surname;
                //applicant.DriversLicence = model.DriversLicence;
                //applicant.DVLAReferenceNo = model.DVLAReferenceNo;
                applicant.FirstName = model.FirstName;
                applicant.OtherName = model.OtherName;
                applicant.DOB = (DateTime)model.DOB;
                applicant.PostalAddress = model.PostalAddress;
                applicant.ContactNumber = model.ContactNumber;
                applicant.Nationality = model.Nationality;
                applicant.Email = model.Email;
                applicant.ResultServiceType = model.ResultServiceType;
                applicant.AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory;
                applicant.PassportImageUrl = model.Filename;
                //applicant.Status = model.Status;
                applicant.TestType = (TestType)model.TestType;
                //applicant.OldDVLAReferenceNo = model.InvoiceNumber;
                applicant.IsActive = model.IsActive;
                applicant.CreatedBy = model.CreatedBy;
                applicant.IsDeleted = model.IsDeleted;
                applicant.ModifiedBy = model.UpdatedBy;
                applicant.Gender = model.Gender;
                _applicantQuery.Update(applicant);


                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.UPPDATE_APPLICANT_REGISTRATION, "Update applicant");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }

        public int GetResultServiceType(string serviceType)
        {
            int resultService = 0;
            switch (serviceType.Trim().ToUpper())
            {
                case "LEARNER DRIVER’S LICENCE":
                    resultService = 1;
                    break;
                case "RENEWAL OF DRIVER’S LICENCE":
                    resultService = 2;
                    break;
                case "REPLACEMENT OF DRIVER’S LICENCE":
                    resultService = 3;
                    break;
                case "UPGRADE OF DRIVER’S LICENCE":
                    resultService = 4;
                    break;
                case "ACCIDENT REPORT":
                    resultService = 5;
                    break;
                default:
                    break;
            }
            return resultService;
        }

        //[HttpPost]
        //public async Task<JsonResult> GetApplicantDetailAsync(string refno)
        //{
        //    ApplicantModel applicant = new ApplicantModel();
        //    try
        //    {


        //        GenesysClient g = new GenesysClient();
        //        var msg = await g.GetApplicantDetail(refno);



        //        if (msg != null && msg.code == "00")
        //        {
        //            string filename = Guid.NewGuid().ToString() + ".png";
        //            string path = Server.MapPath("~/Passports/") + filename;
        //            //flpPassport.PostedFile.SaveAs(Server.MapPath("~/passports/temp/") + filename);

        //            using (WebClient webClient = new WebClient())
        //            {
        //                byte[] dataArr = webClient.DownloadData(msg.data.photo);
        //                //save file to local
        //                var contents = new MemoryStream(dataArr);
        //                BUSINESS.BusinessUtility.Utility.ResizePicture(contents, path);
        //            }

        //            string[] fullname = msg.data.fullName.Split(' ');

        //            byte[] imageArray = System.IO.File.ReadAllBytes(path);


        //            applicant = new ApplicantModel
        //            {
        //                DriversLicence = refno,
        //                Surname = fullname[1],
        //                FirstName = fullname[0],
        //                PassportImageUrl = Convert.ToBase64String(imageArray),
        //                Filename = filename
        //            };



        //            return Json(applicant, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLogManager.Error(ex);
        //    }
        //    return Json(applicant, JsonRequestBehavior.AllowGet);
        //}
    }
}