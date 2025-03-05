using DVLA.Business.NotificationModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}, {AppRoles.SYSTEMADMIN}")]
    public class VisualAssessmentResultController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristFirmUserRepositoryQuery;

        private readonly IRepositoryQuery<VisualAcuityScore> _visualAcuityScoreRepositoryQuery;
        private readonly IRepositoryQuery<VisualFieldScore> _visualFieldScoreRepositoryQuery;
        private readonly IRepositoryQuery<ColourVisionScore> _colourVisionScoreRepositoryQuery;

        private readonly IAuditRepo _AuditRepo;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly IRepositoryQuery<Slot> _slotRepositoryQuery;
        private readonly ISmsRepository _smsRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<VisualAssessmentResultController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly string currentUserId;
        private readonly DVLADbContext _context;
        private readonly IUserService _userService;


        public VisualAssessmentResultController(IRepositoryQuery<OptometristFirmUser> optometristFirmUserRepositoryQuery,
            IRepositoryQuery<OptometristFirm> optometristFirmRepositoryQuery,
            IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultRepositoryQuery,
            IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<ColourVisionScore> colourVisionScoreRepositoryQuery,
            IRepositoryQuery<VisualAcuityScore> visualAcuityScoreRepositoryQuery,
            IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery,
            IUserService userService,
            IRepositoryQuery<Slot> slotRepositoryQuery, ISmsRepository smsRepository, IAuditRepo AuditRepo, INotificationRepository notificationRepository, ILogger<VisualAssessmentResultController> logger, IWebHostEnvironment environment, DVLADbContext context, IConfiguration configuration)
        {
            _userService = userService;
            _visualAcuityScoreRepositoryQuery = visualAcuityScoreRepositoryQuery;
            _visualFieldScoreRepositoryQuery = visualFieldScoreRepositoryQuery;
            _colourVisionScoreRepositoryQuery = colourVisionScoreRepositoryQuery;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _visualAssessmentResultRepositoryQuery = visualAssessmentResultRepositoryQuery;
            _optometristFirmRepositoryQuery = optometristFirmRepositoryQuery;
            _optometristFirmUserRepositoryQuery = optometristFirmUserRepositoryQuery;
            _slotRepositoryQuery = slotRepositoryQuery;
            _smsRepository = smsRepository;
            _AuditRepo = AuditRepo;
            _notificationRepository = notificationRepository;
            currentUserId = userService.GetUserData().Id;
            _logger = logger;
            _environment = environment;
            _context = context;
            _configuration = configuration;
        }

        // GET: Customer/VisualAssessmentResult
        [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]
        [HttpGet]
        public ActionResult Index(int Entries = 10)
        {
            TempData["Action"] = Url.Action("Index", "VisualAssessmentResult", new { area = "Customer" });
            PaginationResponseModel<List<VisualAssessmentResultListItem>> visualAssessmentResults = new();
            try
            {
                ViewBag.StartDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.EndDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.DSReference = string.Empty;
                ViewBag.Status = Status.InProgress.ToString();
                int? OptometristFirmId = _userService.GetUserData().OptometristFirmId;
                PaginationRequestModel pagination = new() { PageSize = Entries };
                visualAssessmentResults = _visualAssessmentResultRepository.GetVisualAssessmentResult(pagination, OptometristFirmId, Status.InProgress, DateTime.Now.AddMonths(-1), DateTime.Now.AddDays(2), null);
                _AuditRepo.AddAudit(Activities.VIEW_VISUAL_ASSESSMENT_RESULT, "ViewVisual Assessment Result");
                return View(visualAssessmentResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(visualAssessmentResults);
        }


        [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]
        [HttpPost]
        public ActionResult Index(DateTime? StartDate, DateTime? EndDate, string DSReference, Status status, int Entries = 10)
        {
            TempData["Action"] = Url.Action("Index", "VisualAssessmentResult", new { area = "Customer" });
            ViewBag.StartDate = StartDate.HasValue ? StartDate.Value.ToString("dd/MM/yyyy") : DateTime.UtcNow.ToString("dd/MM/yyyy");
            ViewBag.EndDate = EndDate.HasValue ? EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd/MM/yyyy") : DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59).ToString("dd/MM/yyyy");
            ViewBag.DSReference = DSReference;
            ViewBag.Status = status.ToString();

            EndDate = EndDate.HasValue ? EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : DateTime.Today.AddHours(23).AddMinutes(59).AddSeconds(59);

            PaginationResponseModel<List<VisualAssessmentResultListItem>> visualAssessmentResults = new();
            try
            {
                int? OptometristFirmId = _userService.GetUserData().OptometristFirmId;
                PaginationRequestModel pagination = new() { PageSize = Entries };
                visualAssessmentResults = _visualAssessmentResultRepository.GetVisualAssessmentResult(pagination, OptometristFirmId, status, StartDate, EndDate.Value, DSReference);
                _AuditRepo.AddAudit(Activities.VIEW_VISUAL_ASSESSMENT_RESULT, "ViewVisual Assessment Result");
                return View(visualAssessmentResults);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(visualAssessmentResults);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(VisualAssessmentResultUploadViewModel model)
        {
            try
            {
                if (model.file == null)
                {
                    model.Errors.Add("No file attached");
                    return View(model);
                }
                string extension = Path.GetExtension(model.file.FileName);
                FileInfo fileInfo = new FileInfo(model.file.FileName);

                if (!extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    model.Errors.Add("Only excel files with .xlsx extension are allowed");
                    return View(model);


                }


                if (!model.file.ContentType.Equals("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                {
                    model.Errors.Add("Invalid file format. Only xlxs files are allowed");
                    return View(model);

                }
                var excelEngine = new ExcelPackage(model.file.OpenReadStream());
                var workBook = excelEngine.Workbook;
                var workSheet = workBook.Worksheets.First();
                var optometristUser = _optometristFirmUserRepositoryQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();

                if (optometristUser == null)
                {
                    model.Errors.Add("Sorry! You have not been mapped to an Optometrist Firm");
                    return View(model);
                }

                var slot = _slotRepositoryQuery.FilterAsync(x => x.OptometristFirmId == optometristUser.OptometristFirmId).Result.FirstOrDefault();
                if (slot == null)
                {
                    model.Errors.Add("There is no available slot to continue with this assessment result");
                    return View(model);
                }
                if (slot.Quantity == 0)
                {
                    model.Errors.Add("There is no available slot to continue with this assessment result");
                    return View(model);
                }

                var list = new List<VisualAssessmentResult>();

                for (int rowIndex = 2; rowIndex <= workSheet.Dimension.End.Row; rowIndex++)
                {
                    //Read data from excel
                    string TestType = workSheet.Cells[rowIndex, 1].Text.Trim().ToUpper();
                    string OldDVLAReferenceNumber = workSheet.Cells[rowIndex, 2].Text.Trim().ToUpper();
                    string ServiceType = workSheet.Cells[rowIndex, 3].Text.Trim().ToUpper();
                    string LicenceNumber = workSheet.Cells[rowIndex, 4].Text.Trim().ToUpper();
                    string DVLAReferenceNumber = workSheet.Cells[rowIndex, 5].Text.Trim().ToUpper();
                    string Title = workSheet.Cells[rowIndex, 6].Text.Trim().ToUpper();
                    string Surname = workSheet.Cells[rowIndex, 7].Text.Trim().ToUpper();
                    string FirstName = workSheet.Cells[rowIndex, 8].Text.Trim().ToUpper();
                    string OtherName = workSheet.Cells[rowIndex, 9].Text.Trim().ToUpper();
                    string DOB = workSheet.Cells[rowIndex, 10].Text.Trim().ToUpper();
                    string PostalAddress = workSheet.Cells[rowIndex, 11].Text.Trim().ToUpper();
                    string ContactNumber = workSheet.Cells[rowIndex, 12].Text.Trim().ToUpper();
                    string TIN = workSheet.Cells[rowIndex, 13].Text.Trim().ToUpper();
                    string Email = workSheet.Cells[rowIndex, 14].Text.Trim().ToUpper();
                    string Unaided_OD = workSheet.Cells[rowIndex, 15].Text.Trim().ToUpper();
                    string Unaided_OS = workSheet.Cells[rowIndex, 16].Text.Trim().ToUpper();
                    string Unaided_OU = workSheet.Cells[rowIndex, 17].Text.Trim().ToUpper();
                    string BCV_OD = workSheet.Cells[rowIndex, 18].Text.Trim().ToUpper();
                    string BCV_OS = workSheet.Cells[rowIndex, 19].Text.Trim().ToUpper();
                    string BCV_OU = workSheet.Cells[rowIndex, 20].Text.Trim().ToUpper();
                    string HX_BCV_OD = workSheet.Cells[rowIndex, 21].Text.Trim().ToUpper();
                    string HX_BCV_OS = workSheet.Cells[rowIndex, 22].Text.Trim().ToUpper();
                    string HX_BCV_OU = (Convert.ToInt32(HX_BCV_OD) + Convert.ToInt32(HX_BCV_OS)).ToString();
                    //string HX_BCV_OU = workSheet.Cells[rowIndex, 23].Text.Trim().ToUpper();
                    string SingleImage_BCV_OU = workSheet.Cells[rowIndex, 24].Text.Trim().ToUpper();
                    string ContrastSensitivity_BCV = workSheet.Cells[rowIndex, 25].Text.Trim().ToUpper();
                    string GlareTest_BCV_OD = workSheet.Cells[rowIndex, 26].Text.Trim().ToUpper();
                    string GlareTest_BCV_OS = workSheet.Cells[rowIndex, 27].Text.Trim().ToUpper();
                    string GlareTest_BCV_OU = workSheet.Cells[rowIndex, 28].Text.Trim().ToUpper();
                    string ColourVision_BCV_OU = workSheet.Cells[rowIndex, 29].Text.Trim().ToUpper();
                    string PathologicalRemarks = workSheet.Cells[rowIndex, 30].Text.Trim().ToUpper();   //required
                    string ResultConclusion = workSheet.Cells[rowIndex, 31].Text.Trim().ToUpper(); //required
                    string PassOrFail = workSheet.Cells[rowIndex, 32].Text.Trim().ToUpper(); //required
                    string PassType = workSheet.Cells[rowIndex, 33].Text.Trim().ToUpper(); //required if pass
                    string LearnerServiceType = workSheet.Cells[rowIndex, 34].Text.Trim().ToUpper();
                    //Validation
                    if (string.IsNullOrEmpty(TestType))
                    {
                        model.Errors.Add("Test Type is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (TestType == "RETEST" && string.IsNullOrWhiteSpace(OldDVLAReferenceNumber))
                    {
                        model.Errors.Add("Old DVLA Reference Number is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(ServiceType))
                    {
                        model.Errors.Add("Service Type is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(LicenceNumber))
                    {
                        model.Errors.Add("Licence Number is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(DVLAReferenceNumber))
                    {
                        model.Errors.Add("DVLA ReferenceNumber is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(Title))
                    {
                        model.Errors.Add("Title is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(Surname))
                    {
                        model.Errors.Add("Surname is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(FirstName))
                    {
                        model.Errors.Add("FirstName is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(PostalAddress))
                    {
                        model.Errors.Add("Postal Address is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(ContactNumber))
                    {
                        model.Errors.Add("Contact Number is required for row: " + rowIndex);
                        return View(model);
                    }

                    //if (string.IsNullOrEmpty(TIN))
                    //{
                    //    TempData["MESSAGE"] = new AlertMessage { Message = "Tax Identification Number (TIN) is required for row: " + rowIndex, MessageType = MessageType.ErrorMessage };
                    //    return RedirectToAction(nameof(Index));
                    //}

                    if (string.IsNullOrEmpty(Unaided_OD))
                    {
                        model.Errors.Add("Unaided OD is required for row: " + rowIndex);
                        return View(model);

                    }

                    if (string.IsNullOrEmpty(Unaided_OS))
                    {
                        model.Errors.Add("Unaided OS is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(Unaided_OU))
                    {
                        model.Errors.Add("Unaided OU is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(BCV_OD))
                    {
                        model.Errors.Add("BCV OD is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(BCV_OS))
                    {
                        model.Errors.Add("BCV OS is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(BCV_OU))
                    {
                        model.Errors.Add("BCV OU is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(HX_BCV_OD))
                    {
                        model.Errors.Add("HX_BCV_OD is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(HX_BCV_OS))
                    {
                        model.Errors.Add("HX_BCV_OS is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(HX_BCV_OU))
                    {
                        model.Errors.Add("HX_BCV_OU is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(PathologicalRemarks))
                    {
                        model.Errors.Add("Pathological Remarks is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(ResultConclusion))
                    {
                        model.Errors.Add("Result Conclusion is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(PassOrFail))
                    {
                        model.Errors.Add("PassOrFail is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (PassOrFail == "PASS" && string.IsNullOrEmpty(PassType))
                    {
                        model.Errors.Add("Pass Type is required for row: " + rowIndex);
                        return View(model);
                    }

                    if (ServiceType.Trim().Equals("Learner Driver's Licence", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(LearnerServiceType))
                        {
                            model.Errors.Add("Learner Driver's Licence is required for row: " + rowIndex);
                            return View(model);
                        }
                    }

                    int optometristFirmId = _optometristFirmUserRepositoryQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault().OptometristFirmId;

                    string referenceNumber = _visualAssessmentResultRepository.GenerateReferenceNo(optometristFirmId);

                    //Add record to list

                    //var newEntry = new VisualAssessmentResult();

                    var serviceType = GetResultServiceType(ServiceType);
                    if (serviceType == 0)
                    {
                        model.Errors.Add("Wrong Service Type uploaded on row: " + rowIndex);
                        return RedirectToAction(nameof(Index));
                    }


                    var newEntry = new VisualAssessmentResult()
                    {
                        NameTitle = Title == "MR" ? NameTitle.Mr : Title == "MRS" ? NameTitle.Mrs : NameTitle.Other,
                        PassOrFail = PassOrFail == "PASS" ? Data.Models.Enumerables.PassOrFail.Pass : Data.Models.Enumerables.PassOrFail.Fail,
                        PassResult = PassType == "UNLIMITED" ? PassResult.Unlimited : PassType == "LIMITED FOR 3 MONTHS" ? PassResult.ThreeMonths : PassResult.SixMonths,
                        Surname = Surname,
                        DriversLicence = LicenceNumber,
                        DVLAReferenceNo = DVLAReferenceNumber,
                        FirstName = FirstName,
                        OtherName = OtherName,
                        DOB = Convert.ToDateTime(DOB),   //(DateTime)DOB.GetDateValue(),
                        PostalAddress = PostalAddress,
                        ContactNumber = ContactNumber,
                        TaxIdentificationNumber = TIN,
                        Email = Email,
                        Unaided_OD = Unaided_OD,
                        Unaided_OS = Unaided_OS,
                        Unaided_OU = Unaided_OU,
                        BCV_OD = BCV_OD,
                        BCV_OS = BCV_OS,
                        BCV_OU = BCV_OU,
                        HX_BCV_OD = HX_BCV_OD,
                        HX_BCV_OS = HX_BCV_OS,
                        HX_BCV_OU = HX_BCV_OU,
                        SingleImage_BCV_OU = SingleImage_BCV_OU,
                        GlareTest_BCV_OD = GlareTest_BCV_OD,
                        GlareTest_BCV_OS = GlareTest_BCV_OS,
                        GlareTest_BCV_OU = GlareTest_BCV_OU,
                        ColourVision_BCV_OU = ColourVision_BCV_OU,
                        //ContrastSensitivity_BCV = ContrastSensitivity_BCV,
                        PathologicalRemarks = PathologicalRemarks,
                        ResultConclusion = ResultConclusion,
                        ResultServiceType = (ResultServiceType)serviceType,// == "DRIVER'S LICENCE" ? ResultServiceType.DriversLicence : ResultServiceType.LearnerDriversLicence,
                        OptometristFirmId = optometristUser.OptometristFirmId,
                        ReferenceNumber = referenceNumber,
                        CreatedBy = currentUserId,
                        IsActive = true,
                        IsDeleted = false,
                        TestDate = DateTime.UtcNow,
                        Status = Status.InProgress,
                        IsSynchronized = false,
                        TestType = TestType == "NEW" ? Data.Models.Enumerables.TestType.NewTest : Data.Models.Enumerables.TestType.ReTest,
                        OldDVLAReferenceNo = OldDVLAReferenceNumber
                    };

                    list.Add(newEntry);

                }

                if (slot.Quantity < list.Count)
                {
                    model.Errors.Add($"You do not have enough slot to upload this resuts. Avaliable slots: {slot.Quantity} No. Of results to upload: {list.Count}");
                    return View(model);
                }

                foreach (var item in list)
                {
                    await _visualAssessmentResultRepositoryQuery.AddAsync(item);
                }

                TempData["SuccessMessage"] = list.Count + " Result(s) added successfuly";
                return RedirectToAction(nameof(Index));


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

        public ActionResult Create(string page, string token = "")
        {
            try
            {
                var visualAcuitys = _visualAcuityScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
                var visualFieldScores = _visualFieldScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
                var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();

                ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
                ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
                ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
                ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
                ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

                long id = 0;

                if (page == "Details")
                {

                    id = long.Parse(Utility.Decrypt(token));

                    var visualAssessmentResults = _visualAssessmentResultRepositoryQuery.FilterAsync(x => x.Id == id).Result.Select(y => new VisualAssessmentResultViewModel()
                    {
                        Id = y.Id,
                        //NameTitle = y.NameTitle,
                        PassOrFail = y.PassOrFail,
                        PassResult = y.PassResult,
                        Surname = y.Surname,
                        DriversLicence = y.DriversLicence,
                        DVLAReferenceNo = y.DVLAReferenceNo,
                        FirstName = y.FirstName,
                        OtherName = y.OtherName,
                        DOB = (DateTime)y.DOB,
                        //DateOfBirth = ((DateTime)y.DOB).ToString("dd-MM-yyyy"),
                        PostalAddress = y.PostalAddress,
                        ContactNumber = y.ContactNumber,
                        TaxIdentificationNumber = y.TaxIdentificationNumber,
                        Email = y.Email,
                        Unaided_OD = y.Unaided_OD,
                        Unaided_OS = y.Unaided_OS,
                        Unaided_OU = y.Unaided_OU,
                        BCV_OD = y.BCV_OD,
                        BCV_OS = y.BCV_OS,
                        BCV_OU = y.BCV_OU,
                        HX_BCV_OD = y.HX_BCV_OD,
                        HX_BCV_OS = y.HX_BCV_OS,
                        HX_BCV_OU = y.HX_BCV_OU,
                        SingleImage_BCV_OU = y.SingleImage_BCV_OU,
                        GlareTest_BCV_OD = y.GlareTest_BCV_OD,
                        GlareTest_BCV_OS = y.GlareTest_BCV_OS,
                        GlareTest_BCV_OU = y.GlareTest_BCV_OU,
                        ColourVision_BCV_OU = y.ColourVision_BCV_OU,
                        //ContrastSensitivity_BCV = y.ContrastSensitivity_BCV,
                        PathologicalRemarks = y.PathologicalRemarks,
                        ResultConclusion = y.ResultConclusion,
                        ResultServiceType = y.ResultServiceType,
                        LearnerDriversLicence = y.LearnerDriversLicence,
                        ReferenceNumber = y.ReferenceNumber,
                        OptometristFirmId = y.OptometristFirmId,
                        PassportImageUrl = y.PassportImageUrl,
                        Status = y.Status,
                        TestType = (byte)y.TestType,
                        OldDVLAReferenceNo = y.OldDVLAReferenceNo,
                        ActionType = "Modify"


                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(visualAssessmentResults.PassportImageUrl) && visualAssessmentResults.PassportImageUrl.Contains(".png"))
                    {
                        var path = Path.Combine(_environment.ContentRootPath, "Passports", visualAssessmentResults.PassportImageUrl);
                        if (System.IO.File.Exists(path))
                        {
                            byte[] imageArray = System.IO.File.ReadAllBytes(path);
                            visualAssessmentResults.PassportImageUrl = Convert.ToBase64String(imageArray);
                        }

                    }


                    return View(visualAssessmentResults);

                }
                else
                {
                    return View(new VisualAssessmentResultViewModel() { Status = Status.InProgress });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            //ColorVisionScores.GetAllList().ToList()
            return View(new VisualAssessmentResultViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Create(VisualAssessmentResultViewModel model)
        {
            try
            {

                var visualAcuitys = _visualAcuityScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var visualFieldScores = _visualFieldScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();

                ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
                ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
                ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
                ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
                ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

                if (model.Action == Status.Complete && string.IsNullOrEmpty(model.ResultConclusion))
                {
                    ModelState.AddModelError("ResultConclusion", "Result Conclusion is required");
                    model.Errors.Add("Result Conclusion is required");
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.Surname))
                {
                    ModelState.AddModelError("Surname", "Please enter surname");
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.FirstName))
                {
                    ModelState.AddModelError("FirstName", "Please enter first name");
                    return View(model);
                }
                if (model.ResultServiceType == null)
                {
                    ModelState.AddModelError("ResultServiceType", "Please select a result service type");
                    return View(model);
                }

                if (string.IsNullOrEmpty(model.ContactNumber))
                {
                    ModelState.AddModelError("ContactNumber", "Please enter contact number");
                    return View(model);
                }
                if (model.DOB == null)
                {
                    ModelState.AddModelError("DOB", "Please select DOB");
                    return View(model);
                }



                string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
                model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;

                bool isSubmitted = model.ActionType != "Modify";

                if (model.ActionType != "Modify")
                {

                    if (string.IsNullOrEmpty(model.PassportImageUrl) && (model.Image.Length == 0 || model.Image == null))
                    {
                        ModelState.AddModelError("PassportImageUrl", "Please capture/upload passport");
                        return View(model);
                    }

                    if (model.ResultServiceType == null)
                    {
                        ModelState.AddModelError("ResultServiceType", "Please select service type");
                        return View(model);
                    }

                    if (model.ResultServiceType != null)
                    {
                        if (model.ResultServiceType == ResultServiceType.LearnerDriversLicence)
                        {
                            if (model.LearnerDriversLicence == null)
                            {
                                ModelState.AddModelError("LearnerDriversLicence", "Please select learner licence type");
                                return View(model);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(model.Unaided_OD))
                    {
                        ModelState.AddModelError("Unaided_OD", "Please select Unaided OD");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.Unaided_OS))
                    {
                        ModelState.AddModelError("Unaided_OS", "Please select Unaided OS");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.BCV_OD))
                    {
                        ModelState.AddModelError("BCV_OD", "Please select BCV OD");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.BCV_OS))
                    {
                        ModelState.AddModelError("BCV_OS", "Please select BCV OS");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.BCV_OU))
                    {
                        ModelState.AddModelError("BCV_OU", "Please select BCV OU");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.HX_BCV_OD))
                    {
                        ModelState.AddModelError("HX_BCV_OD", "Please select HX BCV OD");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.HX_BCV_OS))
                    {
                        ModelState.AddModelError("HX_BCV_OS", "Please select HX BCV OS");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.PathologicalRemarks))
                    {
                        ModelState.AddModelError("PathologicalRemarks", "Please type in your observation(s)");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(model.ResultConclusion))
                    {
                        ModelState.AddModelError("ResultConclusion", "Please type in your observation(s)");
                        return View(model);
                    }
                    else
                    {
                        if (model.PassOrFail == PassOrFail.Pass)
                        {
                            if (model.PassResult == null)
                            {
                                ModelState.AddModelError("PassResultId", "Please select pass type");
                                return View(model);
                            }
                        }
                    }

                    model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);

                    //if (!ModelState.IsValid)
                    //{

                    //    return View(model);
                    //}

                    string filename = Guid.NewGuid().ToString();

                    //model.PassportImageUrl = model.PassportImageUrl.Substring(model.PassportImageUrl.IndexOf(',') + 1);
                    if (model.Image != null && model.Image.Length > 0)
                    {
                        string extension = Path.GetExtension(model.Image.FileName);
                        var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + extension);
                        string directory = Path.GetDirectoryName(path);
                        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                        FileStream fs = new FileStream(path, FileMode.Create);
                        await model.Image.CopyToAsync(fs);

                        model.PassportImageUrl = Path.GetFileName(path);
                    }
                    else if (!string.IsNullOrEmpty(model.PassportImageUrl))
                    {
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
                }

                var optometristUser = _optometristFirmUserRepositoryQuery.FilterAsync(x => x.ApplicationUserId == currentUserId).Result.FirstOrDefault();

                if (optometristUser == null)
                {
                    model.Errors.Add("Sorry! You have not been mapped to an Optometrist Firm");
                    return View(model);
                }

                Slot slot = _slotRepositoryQuery.FilterAsync(x => x.OptometristFirmId == optometristUser.OptometristFirmId && x.AccessType == (model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory)).Result.FirstOrDefault();
                if (slot == null)
                {
                    model.Errors.Add("There is no available slot to continue with this assessment result");
                    return View(model);
                }
                if (slot.Quantity == 0)
                {
                    model.Errors.Add("There is no available slot to continue with this assessment result");
                    return View(model);
                }

                var context = _context;
                var scope = await context.Database.BeginTransactionAsync();

                using (scope)
                {
                    try
                    {
                        if (model.HX_BCV_OD != null && model.HX_BCV_OS != null)
                        {
                            model.HX_BCV_OU = (int.Parse(model.HX_BCV_OD.Trim()) + int.Parse(model.HX_BCV_OS.Trim())).ToString();
                        }

                        if (model.ActionType == "Modify") model.Status = Status.InProgress;
                        else model.Status = Status.Complete;

                        var visualAssessmentResult = await context.VisualAssessmentResults.FirstOrDefaultAsync(x => x.Id == model.Id); //_visualAssessmentResultRepositoryQuery.FilterAsync(x => x.Id == model.Id).Result.FirstOrDefault();
                        if (visualAssessmentResult == null)
                        {
                            if (model.Status == Status.Complete && string.IsNullOrEmpty(model.PassportImageUrl))
                            {
                                await scope.RollbackAsync();
                                model.Errors.Add("Kindly upload Passport");
                                return View(model);
                            }
                            string referenceNumber = _visualAssessmentResultRepository.GenerateReferenceNo(optometristUser.OptometristFirmId);

                            visualAssessmentResult = new VisualAssessmentResult()
                            {
                                //NameTitle = model.NameTitle,
                                PassOrFail = (model.ResultConclusion == "Fit to drive" || model.ResultConclusion == "Fit to drive with glasses") ? PassOrFail.Pass : PassOrFail.Fail,
                                PassResult = model.PassResult,
                                Surname = model.Surname,
                                DriversLicence = model.DriversLicence,
                                DVLAReferenceNo = model.DVLAReferenceNo,
                                FirstName = model.FirstName,
                                OtherName = model.OtherName,
                                DOB = (DateTime)model.DOB,
                                PostalAddress = model.PostalAddress,
                                ContactNumber = model.ContactNumber,
                                TaxIdentificationNumber = model.TaxIdentificationNumber,
                                Email = model.Email,
                                Unaided_OD = model.Unaided_OD,
                                Unaided_OS = model.Unaided_OS,
                                Unaided_OU = model.Unaided_OU,
                                BCV_OD = model.BCV_OD,
                                BCV_OS = model.BCV_OS,
                                BCV_OU = model.BCV_OU,
                                HX_BCV_OD = model.HX_BCV_OD,
                                HX_BCV_OS = model.HX_BCV_OS,
                                HX_BCV_OU = model.HX_BCV_OU,
                                SingleImage_BCV_OU = model.SingleImage_BCV_OU,
                                GlareTest_BCV_OD = model.GlareTest_BCV_OD,
                                GlareTest_BCV_OS = model.GlareTest_BCV_OS,
                                GlareTest_BCV_OU = model.GlareTest_BCV_OU,
                                ColourVision_BCV_OU = model.ColourVision_BCV_OU,
                                //ContrastSensitivity_BCV = model.ContrastSensitivity_BCV,
                                PathologicalRemarks = model.PathologicalRemarks,
                                ResultConclusion = model.ResultConclusion,
                                ResultServiceType = model.ResultServiceType,
                                LearnerDriversLicence = model.LearnerDriversLicence,
                                OptometristFirmId = optometristUser.OptometristFirmId,
                                ReferenceNumber = referenceNumber,
                                CreatedBy = currentUserId,
                                IsActive = true,
                                IsDeleted = false,
                                TestDate = DateTime.UtcNow,
                                PassportImageUrl = model.PassportImageUrl,
                                Status = model.Status,
                                IsSynchronized = false,
                                TestType = (TestType)model.TestType,
                                OldDVLAReferenceNo = model.OldDVLAReferenceNo,
                                IsTransmitted = false,
                                AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory,
                                
                            };
                            //if (model.LearnerDriversLicence != null) visualAssessmentResultCreate.LearnerDriversLicence = model.LearnerDriversLicence;
                            context.VisualAssessmentResults.Add(visualAssessmentResult);
                            await context.SaveChangesAsync();




                            if (model.Status == Status.Complete)
                            {
                                slot.Quantity = slot.Quantity - 1;
                                await context.SaveChangesAsync();

                                //Send Sms Notification
                                string result = model.PassOrFail.ToString(); //EnumHelper<PassOrFail>.GetDisplayValue(model.PassOrFail.GetValueOrDefault());

                                //OnCreateVisualAssessment.Invoke(model, referenceNumber, result, _context);

                                _smsRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, referenceNumber, result, context);
                                //send email
                                _notificationRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, referenceNumber, result, model.Email, context);

                            }

                        }
                        else
                        {

                            string passFile = visualAssessmentResult.PassportImageUrl;
                            if (model.Status == Status.Complete)
                            {
                                if(string.IsNullOrEmpty(passFile) && string.IsNullOrEmpty(model.PassportImageUrl))
                                {
                                    await scope.RollbackAsync();
                                    model.Errors.Add("Kindly upload Passport");
                                    return View(model);
                                }
                            }
                            //visualAssessmentResult.NameTitle = model.NameTitle;
                            visualAssessmentResult.PassOrFail = model.PassOrFail;
                            visualAssessmentResult.PassResult = model.PassResult;
                            visualAssessmentResult.Surname = model.Surname;
                            visualAssessmentResult.DriversLicence = model.DriversLicence;
                            visualAssessmentResult.DVLAReferenceNo = model.DVLAReferenceNo;
                            visualAssessmentResult.FirstName = model.FirstName;
                            visualAssessmentResult.OtherName = model.OtherName;
                            visualAssessmentResult.DOB = (DateTime)model.DOB;
                            visualAssessmentResult.PostalAddress = model.PostalAddress;
                            visualAssessmentResult.ContactNumber = model.ContactNumber;
                            visualAssessmentResult.TaxIdentificationNumber = model.TaxIdentificationNumber;
                            visualAssessmentResult.Email = model.Email;
                            visualAssessmentResult.Unaided_OD = model.Unaided_OD;
                            visualAssessmentResult.Unaided_OS = model.Unaided_OS;
                            visualAssessmentResult.Unaided_OU = model.Unaided_OU;
                            visualAssessmentResult.BCV_OD = model.BCV_OD;
                            visualAssessmentResult.BCV_OS = model.BCV_OS;
                            visualAssessmentResult.BCV_OU = model.BCV_OU;
                            visualAssessmentResult.HX_BCV_OD = model.HX_BCV_OD;
                            visualAssessmentResult.HX_BCV_OS = model.HX_BCV_OS;
                            visualAssessmentResult.HX_BCV_OU = model.HX_BCV_OU;
                            visualAssessmentResult.SingleImage_BCV_OU = model.SingleImage_BCV_OU;
                            visualAssessmentResult.GlareTest_BCV_OD = model.GlareTest_BCV_OD;
                            visualAssessmentResult.GlareTest_BCV_OS = model.GlareTest_BCV_OS;
                            visualAssessmentResult.GlareTest_BCV_OU = model.GlareTest_BCV_OU;
                            visualAssessmentResult.ColourVision_BCV_OU = model.ColourVision_BCV_OU;
                            //visualAssessmentResult.ContrastSensitivity_BCV = model.ContrastSensitivity_BCV;
                            visualAssessmentResult.PathologicalRemarks = model.PathologicalRemarks;
                            visualAssessmentResult.ResultConclusion = model.ResultConclusion;
                            visualAssessmentResult.ResultServiceType = model.ResultServiceType;
                            visualAssessmentResult.LearnerDriversLicence = model.LearnerDriversLicence;
                            visualAssessmentResult.PassportImageUrl = model.PassportImageUrl;
                            visualAssessmentResult.Status = model.Status;
                            visualAssessmentResult.TestType = (TestType)model.TestType;
                            visualAssessmentResult.OldDVLAReferenceNo = model.OldDVLAReferenceNo;
                            visualAssessmentResult.AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory;
                            visualAssessmentResult.IsTransmitted = false;

                            if (model.Status == Status.Complete)
                            {
                                slot.Quantity = slot.Quantity - 1;
                                context.SaveChanges();
                            }
                            //visualAssessmentResult.TestDate = DateTime.UtcNow;
                            await context.SaveChangesAsync();

                            if (!string.IsNullOrEmpty(passFile) && passFile.Contains(".png"))
                            {
                                var deleteFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", passFile);
                                System.IO.File.Delete(deleteFilePath);
                            }


                            //Send Sms Notification
                            if (model.Status == Status.Complete)
                            {
                                string result = model.PassOrFail.ToString(); //EnumHelper<PassOrFail>.GetDisplayValue(model.PassOrFail.GetValueOrDefault());

                                _smsRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, visualAssessmentResult.ReferenceNumber, result, context);
                                //send email
                                _notificationRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, visualAssessmentResult.ReferenceNumber, result, model.Email, context);

                            }
                        }

                        TempData["SuccessMessage"] = "Record saved successfully";
                        _AuditRepo.AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Visual Assessment Result");
                        await scope.CommitAsync();
                        if (model.Status == Status.Complete)
                        {
                            HttpContext.Session.SetString(AppConstants.VISUALASSESSMENTSUBMISSION, JsonConvert.SerializeObject(visualAssessmentResult));
                            return View("AssessmentDetails", visualAssessmentResult);
                        }
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = ex.Message;
                        model.Errors.Add("Kindly try again later");
                        await scope.RollbackAsync();
                        _logger.LogError(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                model.Errors.Add("Kindly try again later");
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult AssessmentDetails()
        {
            VisualAssessmentResult model = new(); model.AccessType = AccessType.LearnerDriversLicence;// { FirstName = "Peter", Surname = "Eikore", OtherName = "Ayebhere", AccessType = AccessType.LearnerDriversLicence, ContactNumber = "08044567632", ReferenceNumber = "189877777779" };
            string visualAssessmentString = HttpContext.Session.GetString(AppConstants.VISUALASSESSMENTSUBMISSION);
            if (string.IsNullOrEmpty(visualAssessmentString))
            {
                return View(model);
            }
            model = JsonConvert.DeserializeObject<VisualAssessmentResult>(visualAssessmentString);
            return View(model);
        }

        public async Task<ActionResult> Detail(string page = "", string token = "")
        {
            //VisualAssessmentPrintResultViewModel model = null;
            VisualAssessmentResultModel model = new();
            try
            {
                //ColorVisionScores.GetAllList().ToList()
                var visualAcuitys = _visualAcuityScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var visualFieldScores = _visualFieldScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();

                ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
                ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
                ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
                ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
                ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

                long id = 0;


                if (page == "Details")
                {
                    id = long.Parse(Utility.Decrypt(token));

                    var result = await _context.VisualAssessmentResults.Include(x => x.OptometristFirm).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id); //await _visualAssessmentResultRepositoryQuery.GetByIdAsync(id);
                    if (result != null)
                    {
                        ApplicationUser optometrist = await _context.ApplicationUsers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == result.CreatedBy);
                        model = new()
                        {
                            Id = id,
                            AccreditationNumber = result.OptometristFirm.AccreditationNumber,
                            BCV_OD = result.BCV_OD,
                            BCV_OS = result.BCV_OS,
                            BCV_OU = result.BCV_OU,
                            BusinessAddress = result.OptometristFirm.BusinessAddress,
                            BusinessName = result.OptometristFirm?.BusinessName,
                            CentreCode = result.OptometristFirm?.CentreCode,
                            ColourVision_BCV_OU = result.ColourVision_BCV_OU,
                            ContactEmail = result.OptometristFirm.ContactEmail,
                            ContactFirstName = result.OptometristFirm?.ContactFirstName,
                            ContactLastName = result.OptometristFirm.ContactLastName,
                            ContactNumber = result.ContactNumber,
                            ContactPhoneNumber = result.OptometristFirm.ContactPhoneNumber,
                            ContrastSensitivity_BCV = result.ContrastSensitivity_BCV,
                            CreatedBy = result.CreatedBy,
                            CreatedByFullName = optometrist.FirstName + " " + optometrist.LastName,
                            CreatedByUsername = optometrist.UserName,
                            DateCreated = result.CreatedDate,
                            DigitalAddress = result.OptometristFirm.DigitalAddress,
                            DistrictName = result.OptometristFirm.District?.Name,
                            DOB = result.DOB,
                            DriversLicence = result.DriversLicence,
                            DVLAReferenceNo = result.DVLAReferenceNo,
                            Email = result.Email,
                            FirstName = result.FirstName,
                            FormNumber = result.FormNumber,
                            GlareTest_BCV_OD = result.GlareTest_BCV_OD,
                            GlareTest_BCV_OS = result.GlareTest_BCV_OS,
                            GlareTest_BCV_OU = result.GlareTest_BCV_OU,
                            HX_BCV_OD = result.HX_BCV_OD,
                            HX_BCV_OS = result.HX_BCV_OS,
                            HX_BCV_OU = result.HX_BCV_OU,
                            IsActive = result.IsActive,
                            IsDeleted = result.IsDeleted,
                            IsGHDriveSynchronized = result.IsSynchronized,
                            IsRegistration = result.IsRegistration,
                            IsSynchronized = result.IsSynchronized,
                            LearnerDriversLicence = result.LearnerDriversLicence,
                            MobileNumber = result.ContactNumber,
                            NameTitle = result.NameTitle,
                            Optometrist = optometrist.FullName,
                            OptometristFirmId = result.OptometristFirmId,
                            OtherName = result.OtherName,
                            PassOrFail = result.PassOrFail,
                            PassportImageUrl = result.PassportImageUrl,
                            PassResult = result.PassResult,
                            PathologicalRemarks = result.PathologicalRemarks,
                            PostalAddress = result.PostalAddress,
                            ReferenceNumber = result.ReferenceNumber,
                            RegionName = result.OptometristFirm.Region?.Name,
                            RegistrationNumber = result.OptometristFirm.RegistrationNumber,
                            ResultConclusion = result.ResultConclusion,
                            ResultServiceType = result.ResultServiceType,
                            SingleImage_BCV_OU = result.SingleImage_BCV_OU,
                            Status = result.Status,
                            Surname = result.Surname,
                            TaxIdentificationNumber = result.TaxIdentificationNumber,
                            TelephoneNumber = result.ContactNumber,
                            TestDate = result.TestDate,
                            Unaided_OD = result.Unaided_OD,
                            Unaided_OS = result.Unaided_OS,
                            Unaided_OU = result.Unaided_OU,
                            UpdatedBy = result.ModifiedBy,
                            UserName = optometrist.UserName,
                            UpdatedByUsername = optometrist.UserName
                        };
                        //var assessmentItem = _visualAssessmentResultRepository.FetchAssessmentResult(result.DriversLicence, result.DVLAReferenceNo, result.ReferenceNumber);
                        if (model != null)
                        {
                            if (!string.IsNullOrEmpty(model.PassportImageUrl))
                            {
                                model.PassportImageUrl = $"{_configuration["AppConstants:BaseUrl"]}/Passports/{model.PassportImageUrl}";
                                //if (model.PassportImageUrl.Contains(".png"))
                                //{
                                //    var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", model.PassportImageUrl);

                                //    byte[] imageArray = System.IO.File.ReadAllBytes(path);
                                //    model.PassportImageUrl = Convert.ToBase64String(imageArray);

                                //}
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "An error occurred while trying to fetch assessment details";
                            return RedirectToAction("Index");
                        }
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            return View(model);
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
        //                //System.IO.File.WriteAllBytes(@path, dataArr);
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
        //                PassportImageUrl = Convert.ToBase64String(imageArray)
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