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
using System.Web;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "System Administrator")]
    public class ReportManagementController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IReportRepository _reportRepository;
        private readonly IRepositoryQuery<Region> _regionQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IVisualAssessmentResultRepository _assessmentResultRepository;
        private readonly IRepositoryQuery<VisualAssessmentResult> _applicantQuery;
        private readonly ILogger<ReportManagementController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserService _userService;
        private readonly IAuthUser _authUser;


        public ReportManagementController(IAuditRepo AuditRepo, IRepositoryQuery<OptometristFirm> optometristFirmQuery, IRepositoryQuery<Region> regionQuery,
            IReportRepository reportRepository, IVisualAssessmentResultRepository assessmentResultRepository, IRepositoryQuery<VisualAssessmentResult> applicantQuery, ILogger<ReportManagementController> logger, IConfiguration configuration, IWebHostEnvironment environment, IUserService userService, IAuthUser authUser)
        {
            _AuditRepo = AuditRepo;
            _regionQuery = regionQuery;
            _reportRepository = reportRepository;
            _optometristFirmQuery = optometristFirmQuery;
            _assessmentResultRepository = assessmentResultRepository;
            _applicantQuery = applicantQuery;
            _logger = logger;
            _configuration = configuration;
            _environment = environment;
            _userService = userService;
            _authUser = authUser;
        }

        // GET: Admin/ReportManagement

        [HttpPost]
        public ActionResult Index()
        {
            var report = new List<SynchronizationReportViewModel>();
            ViewBag.Regions = _regionQuery.GetAll().ToList();
            ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();
            _AuditRepo.AddAudit(Activities.VIEW_REPORT, "View Synchronization Report");
            return View(report);
        }

        [HttpPost]
        public async Task<ActionResult> Index(SynchronizationReportFilterViewModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            var report = new List<SynchronizationReportViewModel>();
            try
            {
                model.IsAdministrator = true;
                ViewBag.Regions = _regionQuery.GetAll().ToList();
                ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();
                report = await _reportRepository.GetSynchronizationReport(model);
                _AuditRepo.AddAudit(Activities.VIEW_REPORT, "View Synchronization Report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(report);
        }



        [HttpPost]
        public ActionResult ExportAudit(List<SynchronizationReportViewModel> model)
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

        [HttpGet]
        public ActionResult ApplicantSearch()
        {
            ClientViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ApplicantSearch(ClientViewModel model)
        {
            List<ClientModel> clients = await _reportRepository.FetchClientSearch(model.SearchParameter, null, null);
            model.Clients = clients;
            return View(model);
        }

        [HttpGet]
        public ActionResult BiodataUpdate(int page=1)
        {
            
            TempData["Action"] = Url.Action("BiodataUpdate", "ReportManagement", new { area = "Admin" });

            PaginationRequestModel<ClientSearchRequest> request = new()
            {
                InputModel = new() { OptometristFirmId = Convert.ToInt32(_authUser.OptometristFirmId), Search = "" },
                PageIndex = page
            };

            PaginationResponseModel<List<VisualAssessmentResultItemViewModel>> model = _assessmentResultRepository.FetchAssessmentResults(request);

            return View(model);
        }

        public ActionResult VisualAssessmentProcess()
        {
            var rq = Request.Headers;
            int displayLength = int.Parse(Request.Query["iDisplayLength"]);
            int displayStart = int.Parse(Request.Query["iDisplayStart"]);

            int sortCol = int.Parse(Request.Query["iSortCol_0"]);
            string sortDir = Request.Query["sSortDir_0"];
            string search = Request.Query["sSearch"];

            ResultViewModel data = _assessmentResultRepository.FetchAssessmentResults(displayLength, displayStart, sortCol, sortDir, search, null);

            var jsonResult = Json(new
            {
                iTotalRecords = data.iTotalRecords,
                iTotalDisplayRecords = data.iTotalDisplayRecords,
                aaData = data.aaData
            });
            return jsonResult;
        }


        public ActionResult VisualAssessmentDetails(string vasReferenceNo)
        {
            var assessment = _assessmentResultRepository.FetchAssessmentResult(vasReferenceNo);
            if (assessment != null)
            {
                if (!string.IsNullOrEmpty(assessment.PassportImageUrl) && assessment.PassportImageUrl.Contains(".png"))
                {
                    var path = Path.Combine(_environment.ContentRootPath, "Passports", assessment.PassportImageUrl);

                    byte[] imageArray = System.IO.File.ReadAllBytes(path);
                    assessment.PassportImageUrl = Convert.ToBase64String(imageArray);
                }
            }
            return View(assessment);
        }

        public ActionResult Update(string Id)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();


                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(Id));
                var applicant = _applicantQuery.Filter(x => x.Id == applicanId).FirstOrDefault();

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
                model.IsRegistration = applicant.IsRegistration;
                model.ReferenceNumber = applicant.ReferenceNumber;

                if (!string.IsNullOrEmpty(applicant.PassportImageUrl) && applicant.PassportImageUrl.Contains(".png"))
                {
                    var path = Path.Combine(_environment.ContentRootPath, "Passports", applicant.PassportImageUrl);

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
        public ActionResult Update(ApplicantModel model, string Id)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();
            try
            {
                if (string.IsNullOrEmpty(model.PassportImageUrl))
                {
                    ModelState.AddModelError("PassportImageUrl", "Please capture/upload passport");
                }


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

                Int64 applicanId = Convert.ToInt64(Utility.Decrypt(Id));
                var applicant = _applicantQuery.Filter(x => x.Id == applicanId).FirstOrDefault();

                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
                model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);


                string base64 = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                byte[] imageBytes = Convert.FromBase64String(base64);
                string filename = Guid.NewGuid().ToString();
                // store the file inside ~/project folder(Img)  
                var path = Path.Combine(_environment.ContentRootPath, "Passports", filename + ".png");
                var contents = new MemoryStream(imageBytes);

                //resize image
                Utility.ResizePicture(contents, path);
                //System.IO.File.WriteAllBytes(path, imageBytes);
                model.PassportImageUrl = filename + ".png";

                string passFile = applicant.PassportImageUrl;

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
                //applicant.Status = model.Status;
                applicant.TestType = (TestType)model.TestType;
                //applicant.OldDVLAReferenceNo = model.InvoiceNumber;
                //applicant.IsActive = model.IsActive;
                //applicant.CreatedBy = model.CreatedBy;
                //applicant.IsDeleted = model.IsDeleted;
                applicant.ModifiedBy = model.UpdatedBy;



                _applicantQuery.Update(applicant);

                if (!string.IsNullOrEmpty(passFile) && passFile.Contains(".png"))
                {
                    var deleteFilePath = Path.Combine(_environment.ContentRootPath, "Passports", passFile);
                    System.IO.File.Delete(deleteFilePath);
                }

                TempData["SuccessMessage"] = "Record saved successfully";
                _AuditRepo.AddAudit(Activities.UPPDATE_APPLICANT_REGISTRATION, "Update applicant");
                return RedirectToAction("BiodataUpdate");
            }
            catch (Exception ex)
            {
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult FetchSlotReductionLogs()
        {
            var optometristFirms = _optometristFirmQuery.GetAll().Select(x => new OptometristFirmModel
            {
                BusinessName = x.BusinessName,
                Id = x.Id
            }).ToList();
            SlotReductionLogViewModel model = new();
            model.OptometristFirms = optometristFirms;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> FetchSlotReductionLogs(SlotReductionLogViewModel model)
        {
            var optometristFirms = _optometristFirmQuery.GetAll().Select(x => new OptometristFirmModel
            {
                BusinessName = x.BusinessName,
                Id = x.Id
            }).ToList();
            model.OptometristFirms = optometristFirms;
            model.SearchParameter.StartDate = model.SearchParameter.StartDate == null ? Utility.StartOfDay(DateTime.UtcNow) : Utility.StartOfDay(model.SearchParameter.StartDate.Value);
            model.SearchParameter.EndDate = model.SearchParameter.EndDate == null ? Utility.EndOfDay(DateTime.UtcNow) : Utility.EndOfDay(model.SearchParameter.EndDate.Value);
            IEnumerable<SlotReductionModel> slotReductions = await _reportRepository.FetchSlotReductionLogs(model.SearchParameter);
            model.SlotReductions = slotReductions;
            return View(model);
        }
    }
}