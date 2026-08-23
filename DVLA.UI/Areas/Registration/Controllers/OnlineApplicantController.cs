using DVLA.Business.LocationModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NPOI.HPSF;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;

namespace DVLA.UI.Areas.Registration.Controllers
{
    [Area("Registration")]
    [AllowAnonymous]
    public class OnlineApplicantController : Controller
    {
        //private readonly IAuditRepo _AuditRepo;       
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultQuery;
        private readonly INotificationRepository _notificationRepository;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IReportRepository _reportRepository;
        private readonly ISmsRepository _smsRepository;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly IRepositoryQuery<Region> _regionQuery;
        private readonly IRepositoryQuery<District> _districtQuery;
        private readonly ILogger<OnlineApplicantController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly ILocationService _locationService;
        private readonly HttpClient client;

        // GET: OnlineApplicant/Applicant
        public OnlineApplicantController(/*IAuditRepo AuditRepo,*/ IRepositoryQuery<VisualAssessmentResult> applicantQuery, IRepositoryQuery<Region> regionQuery,
            INotificationRepository notificationRepository, IReportRepository reportRepository, ISmsRepository smsRepository, IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<OptometristFirm> optometristFirmQuery, IRepositoryQuery<District> districtQuery, ILogger<OnlineApplicantController> logger, IWebHostEnvironment environment, ILocationService locationService)
        {
            //_AuditRepo = AuditRepo;
            _visualAssessmentResultQuery = applicantQuery;
            _notificationRepository = notificationRepository;
            _reportRepository = reportRepository;
            _smsRepository = smsRepository;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _optometristFirmQuery = optometristFirmQuery;
            _districtQuery = districtQuery;
            _regionQuery = regionQuery;
            client = new HttpClient();
            _logger = logger;
            _environment = environment;
            _locationService = locationService;
        }

        // GET: Admin/Optometrist

        [HttpGet]
        public ActionResult Create()
        {
            var countries = _locationService.GetCountries();

            ViewBag.Countries = countries;
            ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().Where(x => x.IsActive).OrderBy(p => p.BusinessName).ToList();
            return View(new ApplicantModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(ApplicantModel model)
        {
            try
            {
                var countries = _locationService.GetCountries();

                ViewBag.Countries = countries;

                ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().Where(x => x.IsActive).OrderBy(p => p.BusinessName).ToList();

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

                //if (string.IsNullOrEmpty(model.IdentityNumber))
                //{
                //    ModelState.AddModelError("IdentityNumber", "Please enter either either Passport Number or National ID");
                //}

                if (model.Gender == null)
                {
                    ModelState.AddModelError("Gender", "Gender is required");
                }

                if (model.ResultServiceType != ResultServiceType.LearnerDriversLicence)
                {
                    if (string.IsNullOrEmpty(model.DvlaLicenseNumber))
                    {
                        model.Errors.Add($"DVLA License Number is required for {EnumHelper.GetDescription(model.ResultServiceType)}");
                        ModelState.AddModelError("DvlaLicenseNumber", $"DVLA License Number is required for {EnumHelper.GetDescription(model.ResultServiceType)}");
                        return View(model);
                    }
                }

                //if (string.IsNullOrEmpty(model.TaxIdentificationNumber))
                //{
                //    ModelState.AddModelError("TaxIdentificationNumber", "Please enter tax identification number");
                //}

                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault();
                    return View(model);
                }




                //var user = UserManager.FindByEmail(model.Email);
                //if (user != null)
                //{
                //    TempData["MESSAGE"] = new AlertMessage { Message = "User already exist", MessageType = MessageType.ErrorMessage };
                //}
                string filename = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                    byte[] imageBytes = Convert.FromBase64String(model.PassportImageUrl);


                    var contents = new MemoryStream(imageBytes);


                    // store the file inside ~/project folder(Img)  
                    var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + ".png");
                    string directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    Utility.ResizePicture(contents, path);
                    System.IO.File.WriteAllBytes(path, imageBytes);
                    model.PassportImageUrl = filename + ".png";
                }
                else if (model.Image != null)
                {
                    bool isValidSize = Utility.ValidatePassport(model.Image);
                    if (!isValidSize)
                    {
                        model.Errors.Add("Your passport photo is too large. Kindly upload a photo that is 120KB or less");
                        TempData["ErrorMessage"] = "Your passport photo is too large. Kindly upload a photo that is 120KB or less";
                        return View(model);
                    }

                    string extension = Path.GetExtension(model.Image.FileName);
                    var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + extension);
                    string directory = Path.GetDirectoryName(path);
                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    FileStream fs = new FileStream(path, FileMode.Create);
                    await model.Image.CopyToAsync(fs);

                    model.PassportImageUrl = Path.GetFileName(path);
                }

                string formNumber = _visualAssessmentResultRepository.GenerateFormNo();

                var applicant = new VisualAssessmentResult()
                {
                    CreatedDate = DateTime.Now,
                    Id = model.Id,
                    //NameTitle = model.NameTitle,
                    Surname = model.Surname,
                    Gender = model.Gender,
                    OptometristFirmId = model.OptometristFirmId,
                    ResultServiceType = model.ResultServiceType,
                    AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory,
                    PassportImageUrl = model.PassportImageUrl,
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
                    CreatedBy = null,
                    //PassportNumber = model.IdentityType == IdentityType.InternationalPassport ? model.IdentityNumber : null,
                    //NationalID = model.IdentityType == IdentityType.NationalIDCard ? model.IdentityNumber : null,
                    DvlaLicenseNumber = model.DvlaLicenseNumber,
                    InvoiceNumber = model.InvoiceNumber,
                    TestExpiryDate = Utility.GetExpiryDate(model.PassResult)
                };

                _visualAssessmentResultQuery.Add(applicant);



                await _smsRepository.SendRegistrationDetail(model.FirstName, model.ContactNumber, formNumber);
                //send email
                await _notificationRepository.SendRegistrationDetail(model.FirstName, model.ContactNumber, formNumber, model.Email);



                TempData["SuccessMessage"] = "Record saved successfully";
                //_AuditRepo.AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Visual Assessment Registration");


                return RedirectToActionPermanent(nameof(Confirmation), new { token = Utility.Encrypt(applicant.Id.ToString()) });


                
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }

        public ActionResult Confirmation(string token)
        {
            try
            {

                //await _smsRepository.SendPendingSms();

                ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();


                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(token));
                var applicant = _visualAssessmentResultQuery.Filter(x => x.Id == applicanId).FirstOrDefault();              

                var model = new VisualAssessmentPrintResultViewModel();
                model.Id = applicant.Id;
                //model.NameTitle = Enum.GetName(typeof(NameTitle), applicant.NameTitle); ;             
                model.Surname = applicant.Surname;
                //model.DriversLicence = applicant.DriversLicence;
                //model.DVLAReferenceNo = applicant.DVLAReferenceNo;
                model.FirstName = applicant.FirstName;
                model.OtherName = applicant.OtherName;
                model.DOB = (DateTime)applicant.DOB;
                model.PostalAddress = applicant.PostalAddress;
                model.ContactNumber = applicant.ContactNumber;
                model.TaxIdentificationNumber = applicant.Nationality;
                model.Email = applicant.Email;
                model.ResultServiceType = Enum.GetName(typeof(ResultServiceType), applicant.ResultServiceType);
                model.PassportImageUrl = applicant.PassportImageUrl;
                model.Status = applicant.Status;
                model.OptometristFirmId = applicant.OptometristFirmId;
                //model.FormNumber = applicant.FormNumber;
                model.TestType = Enum.GetName(typeof(TestType), applicant.TestType); ;
                model.TestExpiryDate = applicant.TestExpiryDate;
                //model.OldDVLAReferenceNo = applicant.OldDVLAReferenceNo;
             


                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                
            }

            return View(new VisualAssessmentPrintResultViewModel());
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

        

     
    }
}
