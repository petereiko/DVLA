using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}, {AppRoles.SYSTEMADMIN}")]
    public class ReportManagementController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IReportRepository _reportRepository;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IVisualAssessmentResultRepository _assessmentResultRepository;
        private readonly IRepositoryQuery<VisualAssessmentResult> _applicantQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly ILogger<ReportManagementController> _logger;
        private readonly string currentUserId;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;


        public ReportManagementController(IAuditRepo AuditRepo, IReportRepository reportRepository,
            IRepositoryQuery<OptometristFirmUser> optometristUserQuery, IUserService userService, IVisualAssessmentResultRepository assessmentResultRepository, IRepositoryQuery<VisualAssessmentResult> applicantQuery, IRepositoryQuery<OptometristFirm> optometristFirmQuery, IAuditRepo auditRepo, ILogger<ReportManagementController> logger, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _AuditRepo = AuditRepo;
            _reportRepository = reportRepository;
            _optometristUserQuery = optometristUserQuery;
            _assessmentResultRepository = assessmentResultRepository;
            _applicantQuery = applicantQuery;
            _optometristFirmQuery = optometristFirmQuery;
            _AuditRepo = auditRepo;
            _userService = userService;
            currentUserId = userService.GetUserData().Id;
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<CustomerReportViewModel> report = new();
            return View(report);
        }

        // GET: Admin/ReportManagement
        [Authorize(Roles = AppRoles.FACILITYOWNER)]
        [HttpPost]
        public async Task<ActionResult> Index(SynchronizationReportFilterViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "ReportManagement", new { area = "Customer" });
            }

            var report = new List<CustomerReportViewModel>();
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            try
            {
                model.IsAdministrator = false;
                model.OptometristFirmId = OptometristFirmId;
                report = await _reportRepository.GetCustomerSynchronizationReport(model);
                model.Reports = report;
                _AuditRepo.AddAudit(Activities.VIEW_REPORT, "View Report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(report);
        }



        [HttpPost]
        public ActionResult ExportAudit(List<CustomerReportViewModel> model)
        {
            if (model != null)
            {
                string fileName = "Synchronization_Report.xlsx";
                var json = JsonConvert.SerializeObject(model);
                byte[] report = _reportRepository.WriteToExcel("xlsx", (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable))));
                return File(report, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]

        public IActionResult ApplicantSearch()
        {
            ClientViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public ActionResult ApplicantSearch(ClientViewModel model)
        {
            if (User.IsInRole(AppRoles.OPTOMETRIST))
            {
                model.Clients = _reportRepository.FetchClientSearch(model.SearchParameter, null, currentUserId).Result.ToList();
            }
            else if (User.IsInRole(AppRoles.FACILITYOWNER))
            {
                model.Clients = _reportRepository.FetchClientSearch(model.SearchParameter, currentUserId, null).Result.ToList();
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult BiodataUpdate(int Entries = 10)
        {
            TempData["Action"] = Url.Action("BiodataUpdate", "ReportManagement", new { area = "Customer" });

            

            PaginationRequestModel<ClientSearchRequest> request = new()
            {
                InputModel = new() { OptometristFirmId = _userService.GetUserData().OptometristFirmId, Search = "" },
                PageIndex = Entries
            };
            ViewBag.Entries = Entries.ToString();
            ViewBag.StartDate = request.InputModel.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = request.InputModel.EndDate.ToString("dd/MM/yyyy");
            PaginationResponseModel<List<VisualAssessmentResultItemViewModel>> visualAssessments = _assessmentResultRepository.FetchAssessmentResults(request);

            return View(visualAssessments);
        }

        [HttpPost]
        public ActionResult BiodataUpdate(ClientSearchRequest model)
        {
            ViewBag.Entries = model.Entries.ToString();
            PaginationRequestModel<ClientSearchRequest> request = new()
            {
                InputModel = new() { OptometristFirmId = _userService.GetUserData().OptometristFirmId, Search = "", StartDate = model.StartDate, EndDate = model.EndDate },
                PageSize = model.Entries
            };
            ViewBag.StartDate = request.InputModel.StartDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = request.InputModel.EndDate.ToString("dd/MM/yyyy");
            PaginationResponseModel<List<VisualAssessmentResultItemViewModel>> result = _assessmentResultRepository.FetchAssessmentResults(request);
            return View(result);
        }

        [HttpGet]
        public IActionResult VisualAssessmentProcess()
        {
            var rq = Request.Headers;
            int displayLength = int.Parse(Request.Query["iDisplayLength"]);
            int displayStart = int.Parse(Request.Query["iDisplayStart"]);

            int sortCol = int.Parse(Request.Query["iSortCol_0"]);
            string sortDir = Request.Query["sSortDir_0"];
            string search = Request.Query["sSearch"];
            var userData = _userService.GetUserData();
            int? OptometristFirmId = userData.OptometristFirmId;
            ResultViewModel data = _assessmentResultRepository.FetchAssessmentResults(displayLength, displayStart, sortCol, sortDir, search, OptometristFirmId);



            var jsonResult = Json(new
            {
                iTotalRecords = data.iTotalRecords,
                iTotalDisplayRecords = data.iTotalDisplayRecords,
                aaData = data.aaData
            });
            return jsonResult;


        }

        public IActionResult VisualAssessmentDetails(string vasReferenceNo)
        {
            VisualAssessmentResultModel model = new VisualAssessmentResultModel();
            var assessments = _assessmentResultRepository.FetchAssessmentResult(vasReferenceNo);
            if (assessments != null)
            {
                if (!string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    model.PassportImageUrl = $"{_configuration["AppConstants:BaseUrl"]}/Passports/{model.PassportImageUrl}";
                }
            }
            return View(assessments);
        }


        public IActionResult Update(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                ViewBag.OptometristFirms = _optometristFirmQuery.GetAllAsync().Result.ToList();


                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(token));
                var applicant = _applicantQuery.Filter(x => x.Id == applicanId).FirstOrDefault();
                var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == currentUserId, x => x.OptometristFirm).FirstOrDefault();
                int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
                //var firm = _optometristFirmQuery.Filter(x => x.Id == OptometristFirmId).FirstOrDefault();

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
                model.TestType = applicant.TestType;
                //model.InvoiceNumber = applicant.OldDVLAReferenceNo;
                model.Optometrist = optometristUser == null ? "" : optometristUser.OptometristFirm.BusinessName;
                model.IsActive = applicant.IsActive;
                model.CreatedBy = applicant.CreatedBy;
                model.IsDeleted = applicant.IsDeleted;
                model.UpdatedBy = applicant.ModifiedBy;
                model.IsRegistration = applicant.IsRegistration;
                model.ReferenceNumber = applicant.ReferenceNumber;

                if (!string.IsNullOrEmpty(applicant.PassportImageUrl) && applicant.PassportImageUrl.Contains(".png"))
                {
                    var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", applicant.PassportImageUrl);

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
        public async Task<IActionResult> Update(ApplicantModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            ViewBag.OptometristFirms = (await _optometristFirmQuery.GetAllAsync()).ToList();
            try
            {
                if (model.ResultServiceType == null)
                {
                    ModelState.AddModelError("ResultServiceType", "Please select service type");
                    TempData["ErrorMessage"] = "Please select service type";
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.Surname))
                {
                    ModelState.AddModelError("Surname", "Please enter surname");
                    TempData["ErrorMessage"] = "Please enter surname";
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.FirstName))
                {
                    ModelState.AddModelError("FirstName", "Please enter first name");
                    TempData["ErrorMessage"] = "Please enter first name";
                    return View(model);
                }

                if (model.DOB == null)
                {
                    ModelState.AddModelError("DOB", "Please select DOB");
                    TempData["ErrorMessage"] = "Please select DOB";
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.PostalAddress))
                {
                    ModelState.AddModelError("PostalAddress", "Please enter postal address");
                    TempData["ErrorMessage"] = "Please enter postal address";
                    return View(model);
                }

                if (model.OptometristFirmId == 0)
                {
                    ModelState.AddModelError("OptometristFirmId", "Please select Optometrist Firm");
                    TempData["ErrorMessage"] = "Please select Optometrist Firm";
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.ContactNumber))
                {
                    ModelState.AddModelError("ContactNumber", "Please enter contact number");
                    TempData["ErrorMessage"] = "Please enter contact number";
                    return View(model);
                }

                //if (string.IsNullOrEmpty(model.TaxIdentificationNumber))
                //{
                //    ModelState.AddModelError("TaxIdentificationNumber", "Please enter tax identification number");
                //}

                //if (!ModelState.IsValid)
                //{

                //    string error = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault();
                //    TempData["ErrorMessage"] = error;
                //    return View(model);
                //}

                Int64 applicanId = model.Id;
                var applicant = _applicantQuery.Filter(x => x.Id == applicanId).FirstOrDefault();

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                //model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;

                string filename = Guid.NewGuid().ToString();
                string path = string.Empty;
                byte[] imageBytes = new byte[128];
                MemoryStream contents = null;

                if (model.Image != null || !string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    if (model.Image != null && model.Image.Length > 0)
                    {
                        string extension = Path.GetExtension(model.Image.FileName);
                        path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + extension);
                        string directory = Path.GetDirectoryName(path);
                        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                        FileStream fs = new FileStream(path, FileMode.Create);
                        await model.Image.CopyToAsync(fs);

                        model.PassportImageUrl = Path.GetFileName(path);
                    }
                    else if (!string.IsNullOrEmpty(model.PassportImageUrl))
                    {
                        string base64 = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                        imageBytes = Convert.FromBase64String(base64);


                        contents = new MemoryStream(imageBytes);


                        // store the file inside ~/project folder(Img)  

                        path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + ".png");
                        string directory = Path.GetDirectoryName(path);
                        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                        Utility.ResizePicture(contents, path);
                        System.IO.File.WriteAllBytes(path, imageBytes);
                        model.PassportImageUrl = filename + ".png";
                    }
                }


                //model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);


                //string base64 = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                //byte[] imageBytes = Convert.FromBase64String(base64);
                //string filename = Guid.NewGuid().ToString();
                // store the file inside ~/project folder(Img)  
                //var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + ".png");
                //var contents = new MemoryStream(imageBytes);

                //resize image
                //Utility.ResizePicture(contents, path);
                //System.IO.File.WriteAllBytes(path, imageBytes);
                //model.PassportImageUrl = filename + ".png";
                //string passFile = applicant.PassportImageUrl;
                var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == currentUserId, x => x.OptometristFirm).FirstOrDefault();
                int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

                //var firm = _optometristFirmQuery.FilterAsync(x => x.Id == OptometristFirmId).Result.FirstOrDefault();


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
                if (model.Image != null || !string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    applicant.PassportImageUrl = model.PassportImageUrl;
                }
                //applicant.Status = model.Status;
                applicant.TestType = (TestType)model.TestType;
                //applicant.OldDVLAReferenceNo = model.InvoiceNumber;
                //applicant.IsActive = model.IsActive;
                //applicant.CreatedBy = model.CreatedBy;
                //applicant.IsDeleted = model.IsDeleted;
                applicant.ModifiedBy = model.UpdatedBy;
                model.Optometrist = optometristUser == null ? "" : optometristUser.OptometristFirm.BusinessName;

                await _applicantQuery.UpdateAsync(applicant);

                if (!string.IsNullOrEmpty(model.PassportImageUrl) && model.PassportImageUrl.Contains(".png"))
                {
                    var deleteFilePath = Path.Combine(_environment.ContentRootPath, "Passports", model.PassportImageUrl);
                    if (System.IO.File.Exists(deleteFilePath)) System.IO.File.Delete(deleteFilePath);
                }

                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.UPPDATE_APPLICANT_REGISTRATION, "Update applicant");
                return RedirectToAction("BiodataUpdate");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kindly try again later";
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }

    }
}