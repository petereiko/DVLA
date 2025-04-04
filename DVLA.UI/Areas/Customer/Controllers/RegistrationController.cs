
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]
    public class RegistrationController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IRepositoryQuery<VisualAssessmentResult> _applicantQuery;
        private readonly IRepositoryQuery<Region> _regionQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristFirmUserQuery;
        private readonly IReportRepository _reportRepository;
        private readonly ISmsRepository _smsRepository;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly ILogger<RegistrationController> _logger;
        private readonly IAuthUser _authUser;

        // GET: VisualAssessmentResult/Applicant
        public RegistrationController(IAuditRepo AuditRepo, IRepositoryQuery<VisualAssessmentResult> applicantQuery, IRepositoryQuery<Region> regionQuery,
            INotificationRepository notificationRepository, IReportRepository reportRepository, ISmsRepository smsRepository, IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<OptometristFirm> optometristFirmQuery, IAuditRepo auditRepo, IUserService userService, ILogger<RegistrationController> logger, IRepositoryQuery<OptometristFirmUser> optometristFirmUserQuery, IAuthUser authUser)
        {
            _AuditRepo = AuditRepo;
            _applicantQuery = applicantQuery;
            _regionQuery = regionQuery;
            _notificationRepository = notificationRepository;
            _reportRepository = reportRepository;
            _smsRepository = smsRepository;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _optometristFirmQuery = optometristFirmQuery;
            _AuditRepo = auditRepo;
            _logger = logger;
            _optometristFirmUserQuery = optometristFirmUserQuery;
            _authUser = authUser;
        }

        // GET: Admin/Optometrist
        public ActionResult Index()
        {

            try
            {
                OptometristFirmUser optometristFirmUser = _optometristFirmUserQuery.Filter(x => x.ApplicationUserId == _authUser.UserId).FirstOrDefault();
                int OptometristFirmId = optometristFirmUser == null ? 0 : optometristFirmUser.OptometristFirmId;
                //var obj2 = _applicantQuery.GetAll().ToList();
                var obj = _applicantQuery.GetAllAsync().Result.Join(_optometristFirmQuery.GetAllAsync().Result,
                    a => a.OptometristFirmId,
                    o => o.Id, (a, o) => new { a, o }).Where(x => x.a.IsRegistration == true && x.a.OptometristFirmId == OptometristFirmId)
                    .Select(p => new ApplicantModel
                    {
                        ContactNumber = p.a.ContactNumber,
                        FirstName = p.o.BusinessAddress,
                        DOB = p.a.DOB,
                        Email = p.a.Email,
                        OtherName = p.a.OtherName,
                        CreatedBy = p.o.CreatedBy,
                        Optometrist = p.o.BusinessName,
                        OptometristFirmId = p.o.Id,
                        PostalAddress = p.a.PostalAddress,
                        Surname = p.a.Surname,
                        Nationality = p.a.Nationality,
                        Id = p.a.Id,
                        IsActive = p.o.IsActive,
                        IsDeleted = p.o.IsDeleted,
                        UpdatedBy = p.o.ModifiedBy
                    }).ToList();
                    _AuditRepo.AddAudit(Activities.APPLICANT_REGISTRATION, "View Applicant List");
                return View(obj);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kindly try again later";
                _logger.LogError(ex.Message, ex);
            }
            return View(new List<ApplicantModel>());
        }

        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
            }
            ViewBag.OptometristFirms = _optometristFirmQuery.GetAllAsync().Result.ToList();
            return View(new ApplicantModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(ApplicantModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
                }
                ViewBag.OptometristFirms = _optometristFirmQuery.GetAllAsync().Result.ToList();

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
                
                

                if (model.ResultServiceType == null)
                {
                    ModelState.AddModelError("ResultServiceType", "Please select service type");
                }

                if (string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    ModelState.AddModelError("PassportImageUrl", "Please capture/upload passport");
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

                if (string.IsNullOrEmpty(model.Nationality))
                {
                    ModelState.AddModelError("TaxIdentificationNumber", "Please enter tax identification number");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }




                //var user = UserManager.FindByEmail(model.Email);
                //if (user != null)
                //{
                //    TempData["MESSAGE"] = new AlertMessage { Message = "User already exist", MessageType = MessageType.ErrorMessage };
                //}

                model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);

                string referenceNumber = _visualAssessmentResultRepository.GenerateFormNo();

                var applicant = new VisualAssessmentResult()
                {
                    Id = model.Id,
                    Surname = model.Surname,
                    OptometristFirmId = model.OptometristFirmId,
                    ResultServiceType = model.ResultServiceType,
                    PassportImageUrl = model.PassportImageUrl,
                    TestType = model.TestType,
                    Status = Status.InProgress,
                    FirstName = model.FirstName,
                    OtherName = model.OtherName,
                    DOB = (DateTime)model.DOB,
                    PostalAddress = model.PostalAddress,
                    ContactNumber = model.ContactNumber,
                    //FormNumber = referenceNumber,
                    Nationality = model.Nationality,
                    Email = model.Email,
                    IsRegistration = true,
                    CreatedBy = _authUser.UserId,
                };

                await _applicantQuery.AddAsync(applicant);



                await _smsRepository.SendRegistrationDetail(model.FirstName, model.ContactNumber, referenceNumber);
                //send email
                await _notificationRepository.SendRegistrationDetail(model.FirstName, model.ContactNumber, referenceNumber, model.Email);



                TempData["SuccessMessage"] = "Record saved successfully";
               _AuditRepo.AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Visual Assessment Registration");

                //await _smsRepository.SendPendingSms();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kindly try again later";
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }


        public ActionResult Update(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
                }
                ViewBag.OptometristFirms = _optometristFirmQuery.GetAllAsync().Result.ToList();


                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(Id));
                var applicant = _applicantQuery.FilterAsync(x => x.Id == applicanId).Result.FirstOrDefault();              

                var model = new ApplicantModel();
                model.Id = applicant.Id;
                //model.NameTitle = applicant.NameTitle;             
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
                model.PassportImageUrl = applicant.PassportImageUrl;
                model.Status = applicant.Status;
                model.OptometristFirmId = applicant.OptometristFirmId;
                //model.FormNumber = applicant.FormNumber;
                model.TestType = (TestType)applicant.TestType;
                //model.InvoiceNumber = applicant.OldDVLAReferenceNo;
                model.IsActive = applicant.IsActive;
                model.CreatedBy = applicant.CreatedBy;
                model.IsDeleted = applicant.IsDeleted;
                model.UpdatedBy = applicant.ModifiedBy;


                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return View(new ApplicantModel());
        }
             
        [HttpPost]
        public ActionResult Update(ApplicantModel model, string Id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Customer" });
            }
            ViewBag.OptometristFirms = _optometristFirmQuery.GetAllAsync().Result.ToList();
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

                if (string.IsNullOrEmpty(model.Nationality))
                {
                    ModelState.AddModelError("TaxIdentificationNumber", "Please enter tax identification number");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(Id));
                var applicant = _applicantQuery.FilterAsync(x => x.Id == applicanId).Result.FirstOrDefault();

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
                model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);




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
                applicant.PassportImageUrl = model.PassportImageUrl;
                applicant.Status = model.Status;
                applicant.TestType = (TestType)model.TestType;
                //applicant.OldDVLAReferenceNo = model.InvoiceNumber;
                applicant.IsActive = model.IsActive;
                applicant.CreatedBy = model.CreatedBy;
                applicant.IsDeleted = model.IsDeleted;
                applicant.ModifiedBy = model.UpdatedBy;
                _applicantQuery.UpdateAsync(applicant);


                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.UPPDATE_APPLICANT_REGISTRATION, "Update applicant");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kindly try again later";
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
        //public async Task<JsonResult> GetApplicantDetail(string refno)
        //{
        //    ApplicantModel applicant = null;
        //    try
        //    {
        //        DVLAServiceReference.FLTDVLARecord driverInfo = new DVLAServiceReference.FLTDVLARecord();
        //        DVLAServiceReference.StudentAttendanceData studentDetail = new DVLAServiceReference.StudentAttendanceData();
                
        //        using (DVLAServiceReference.DVLAServiceSoapClient client = new DVLAServiceReference.DVLAServiceSoapClient(DVLAServiceSoapClient.EndpointConfiguration.DVLAServiceSoap))
        //        {
        //            studentDetail = await client.FetchStudentAttendanceDetailsAsync(refno);
        //        }

        //        if(studentDetail.ReferenceNo != null)
        //        {
        //            var dd = studentDetail.DriverInfo;
        //            applicant = new ApplicantModel
        //            {
        //                DriversLicence = "",
        //                DVLAReferenceNo = studentDetail.DVLAReferenceNo,
        //                Surname = studentDetail.DriverInfo.surname,
        //                FirstName = studentDetail.DriverInfo.firstname,
        //                OtherName = studentDetail.DriverInfo.middlename,
        //                ContactNumber = studentDetail.DriverInfo.telephone,
        //                Email = studentDetail.EmailAddress,   
        //                PostalAddress = studentDetail.DriverInfo.contactaddress

        //            };

                  
        //            return Json(applicant);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message, ex);
        //    }
        //    return Json(applicant);
        //}
    }
}