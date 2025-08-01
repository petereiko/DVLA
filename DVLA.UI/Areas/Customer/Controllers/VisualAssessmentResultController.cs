using DVLA.Business.EmailModule;
using DVLA.Business.LocationModule;
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
using static System.Formats.Asn1.AsnWriter;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}, {AppRoles.FRONTOFFICER}, {AppRoles.SYSTEMADMIN}")]
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
        private readonly DVLADbContext _context;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ILocationService _locationService;
        private readonly IAuthUser _authUser;


        public VisualAssessmentResultController(IRepositoryQuery<OptometristFirmUser> optometristFirmUserRepositoryQuery,
            IRepositoryQuery<OptometristFirm> optometristFirmRepositoryQuery,
            IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultRepositoryQuery,
            IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<ColourVisionScore> colourVisionScoreRepositoryQuery,
            IRepositoryQuery<VisualAcuityScore> visualAcuityScoreRepositoryQuery,
            IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery,
            IUserService userService,
            IRepositoryQuery<Slot> slotRepositoryQuery, ISmsRepository smsRepository, IAuditRepo AuditRepo, INotificationRepository notificationRepository, ILogger<VisualAssessmentResultController> logger, IWebHostEnvironment environment, DVLADbContext context, IConfiguration configuration, IEmailService emailService, ILocationService locationService, IAuthUser authUser)
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
            _logger = logger;
            _environment = environment;
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _locationService = locationService;
            _authUser = authUser;
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
                int? OptometristFirmId = _authUser.OptometristFirmId;
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
                int? OptometristFirmId = _authUser.OptometristFirmId;
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
                var optometristUser = _optometristFirmUserRepositoryQuery.Filter(x => x.ApplicationUserId == _authUser.UserId).FirstOrDefault();

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
                    string NationalID = workSheet.Cells[rowIndex,35].Text.Trim().ToUpper();
                    string PassportNumber = workSheet.Cells[rowIndex, 36].Text.Trim().ToUpper();
                    string DvlaLicenseNumber = workSheet.Cells[rowIndex, 37].Text.Trim().ToUpper();
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

                    if (string.IsNullOrEmpty(NationalID) && string.IsNullOrEmpty(PassportNumber))
                    {
                        model.Errors.Add("National ID or Password Number is required for row: " + rowIndex);
                        return View(model);
                    }


                    int optometristFirmId = _optometristFirmUserRepositoryQuery.Filter(x => x.ApplicationUserId == _authUser.UserId).FirstOrDefault().OptometristFirmId;

                    string referenceNumber = _visualAssessmentResultRepository.GenerateReferenceNo(optometristFirmId, Status.Complete);

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
                        //NameTitle = Title == "MR" ? NameTitle.Mr : Title == "MRS" ? NameTitle.Mrs : NameTitle.Other,
                        PassOrFail = PassOrFail == "PASS" ? Data.Models.Enumerables.PassOrFail.Pass : Data.Models.Enumerables.PassOrFail.Fail,
                        PassResult = PassType == "UNLIMITED" ? PassResult.Unlimited : PassType == "LIMITED FOR 3 MONTHS" ? PassResult.ThreeMonths : PassResult.SixMonths,
                        Surname = Surname,
                        //DriversLicence = LicenceNumber,
                        //DVLAReferenceNo = DVLAReferenceNumber,
                        FirstName = FirstName,
                        OtherName = OtherName,
                        DOB = Convert.ToDateTime(DOB),   //(DateTime)DOB.GetDateValue(),
                        PostalAddress = PostalAddress,
                        ContactNumber = ContactNumber,
                        Nationality = TIN,
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
                        CreatedBy = _authUser.UserId,
                        IsActive = true,
                        IsDeleted = false,
                        TestDate = DateTime.UtcNow,
                        Status = Status.InProgress,
                        IsSynchronized = false,
                        TestType = TestType == "NEW" ? Data.Models.Enumerables.TestType.NewTest : Data.Models.Enumerables.TestType.ReTest,
                        NationalID = NationalID,
                        PassportNumber = PassportNumber,
                        DvlaLicenseNumber = DvlaLicenseNumber
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

        public async Task<ActionResult> Create(string page, string token = "")
        {
            ViewBag.Token = token;
            try
            {
                VisualAssessmentResultViewModel model = new();

                var visualAcuitys = _visualAcuityScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
                var visualFieldScores = _visualFieldScoreRepositoryQuery.Filter(x => x.IsActive).ToList();
                var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();
                var countries = _locationService.GetCountries();

                

                ViewBag.Countries = countries;

                ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
                ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
                ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
                ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
                ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

                long id = 0;

                if (page == "Details")
                {

                    id = long.Parse(Utility.Decrypt(token));

                    model = (await _visualAssessmentResultRepositoryQuery.FilterAsync(x => x.Id == id)).Select(y => new VisualAssessmentResultViewModel()
                    {
                        Id = y.Id,
                        PassOrFail = y.PassOrFail,
                        PassResult = y.PassResult,
                        Surname = y.Surname,
                        FirstName = y.FirstName,
                        OtherName = y.OtherName,
                        DOB = (DateTime?)y.DOB,
                        PostalAddress = y.PostalAddress,
                        ContactNumber = y.ContactNumber,
                        Nationality = y.Nationality,
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
                        ReferenceNumber = y.ReferenceNumber,
                        OptometristFirmId = y.OptometristFirmId,
                        PassportImageUrl = y.PassportImageUrl,
                        Status = y.Status,
                        TestType = y.TestType,
                        ActionType = "Modify",
                        Gender = y.Gender,
                        DvlaLicenseNumber = y.DvlaLicenseNumber,
                        IdentityNumber = string.IsNullOrEmpty(y.PassportNumber) ? y.NationalID : y.PassportNumber,
                        IdentityType = string.IsNullOrEmpty(y.PassportNumber) ? IdentityType.NationalIDCard : IdentityType.InternationalPassport
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(model.PassportImageUrl) && model.PassportImageUrl.Contains(".png"))
                    {
                        var path = Path.Combine(_environment.ContentRootPath, "Passports", model.PassportImageUrl);
                        if (System.IO.File.Exists(path))
                        {
                            byte[] imageArray = System.IO.File.ReadAllBytes(path);
                            model.PassportImageUrl = Convert.ToBase64String(imageArray);
                        }
                    }
                    model.PassOrFailInt = model.PassOrFail != null ? (int)model.PassOrFail : null;
                    return View(model);

                }
                else
                {
                    model.Status = Status.InProgress;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            //ColorVisionScores.GetAllList().ToList()
            return View(new VisualAssessmentResultViewModel());
        }
       

        private bool ValidateModelState(VisualAssessmentResultViewModel model)
        {
            bool result = true;
            if (string.IsNullOrEmpty(model.ResultConclusion))
            {
                ModelState.AddModelError("ResultConclusion", "Result Conclusion is required");
                result = false;
            }
            if (model.Gender == null)
            {
                ModelState.AddModelError("Gender", "Gender is required");
                result = false;
            }
            if (model.Id > 0)
            {
                VisualAssessmentResult visualAssessmentResult = _visualAssessmentResultRepositoryQuery.GetById(model.Id);
                if (string.IsNullOrEmpty(visualAssessmentResult.PassportImageUrl) && model.Action == Status.Complete && string.IsNullOrEmpty(model.PassportImageUrl) && (model.Image==null|| model.Image.Length==0))
                {
                    ModelState.AddModelError("PassportImageUrl", "Kindly take a Snapshot or upload a Passport");
                    ModelState.AddModelError("Image", "Kindly take a Snapshot or upload a Passport");
                    result = false;
                }
                if (model.DOB == null)
                {
                    ModelState.AddModelError("DOB", "DOB is required");
                    result = false;
                }
            }
            else
            {
                if (model.Action == Status.Complete && string.IsNullOrEmpty(model.PassportUploadType) && model.Image==null)
                {
                    ModelState.AddModelError("PassportImageUrl", "Kindly take a Passport Picture");
                    ModelState.AddModelError("Image", "Kindly upload a Passport Picture");
                    result = false;
                }
            }
            if (model.PassOrFail == null)
            {
                ModelState.AddModelError("PassOrFail", "PassOrFail is required");
                result = false;
            }
            if (model.PassOrFail != null)
            {
                if (model.PassOrFail == PassOrFail.Pass && model.ResultConclusion == "Not fit to drive")
                {
                    ModelState.AddModelError("PassOrFail", "Result Conclusion cannot be 'Not fit to drive and PassOrFail is Pass'");
                    result = false;
                }
            }
            if (string.IsNullOrEmpty(model.Surname))
            {
                ModelState.AddModelError("Surname", "Surname is required");
                result = false;
            }

            if (string.IsNullOrEmpty(model.FirstName))
            {
                ModelState.AddModelError("FirstName", "First Name is required");
                result = false;
            }
            if (string.IsNullOrEmpty(model.ContactNumber))
            {
                ModelState.AddModelError("ContactNumber", "Contact Number is required");
                result = false;
            }
            if (model.DOB == null)
            {
                ModelState.AddModelError("DOB", "DOB is required");
                result = false;
            }
            //string[] dob = model.DateOfBirth != null ? model.DateOfBirth.Split('-') : null;
            //model.DOB = dob != null ? new DateTime(Convert.ToInt32(dob[0]), Convert.ToInt32(dob[1]), Convert.ToInt32(dob[2])) : model.DOB;
            //bool isSubmitted = model.ActionType != "Modify";
            //bool useSlot = isSubmitted;
            if (string.IsNullOrEmpty(model.Nationality))
            {
                ModelState.AddModelError("Nationality", "Nationality is required");
                result = false;
            }
            if (string.IsNullOrEmpty(model.Unaided_OD))
            {
                ModelState.AddModelError("Unaided_OD", "Unaided_OD is required");
                result = false;
            }
            if (string.IsNullOrEmpty(model.Unaided_OS))
            {
                ModelState.AddModelError("Unaided_OS", "Unaided_OS is required");
                result = false;
            }
            if (string.IsNullOrEmpty(model.BCV_OD))
            {
                ModelState.AddModelError("BCV_OD", "Please select BCV OD");
                result = false;
            }
            if (string.IsNullOrEmpty(model.BCV_OS))
            {
                ModelState.AddModelError("BCV_OS", "Please select BCV_OS");
                result = false;
            }
            if (string.IsNullOrEmpty(model.BCV_OU))
            {
                ModelState.AddModelError("BCV_OU", "Please select BCV OU");
                result = false;
            }
            if (string.IsNullOrEmpty(model.HX_BCV_OD))
            {
                ModelState.AddModelError("HX_BCV_OD", "Please select HX_BCV_OD");
                result = false;
            }
            if (string.IsNullOrEmpty(model.HX_BCV_OS))
            {
                ModelState.AddModelError("HX_BCV_OS", "Please select HX_BCV_OS");
                result = false;
            }
            if (string.IsNullOrEmpty(model.PathologicalRemarks))
            {
                ModelState.AddModelError("PathologicalRemarks", "Please select Pathological Remarks");
                result = false;
            }
            if (string.IsNullOrEmpty(model.ResultConclusion))
            {
                ModelState.AddModelError("ResultConclusion", "Please select ResultConclusion");
                result = false;
            }
            if (model.ResultServiceType == null)
            {
                ModelState.AddModelError("ResultServiceType", "Please select ResultServiceType");
                result = false;
            }
            return result;
        }


        public async Task<string> SaveImage(VisualAssessmentResultViewModel model)
        {

            if (string.IsNullOrEmpty(model.PassportUploadType))
                return null;
            string filename = Guid.NewGuid().ToString();

            if (model.PassportUploadType == "WebCam")
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
            else
            {
                string extension = Path.GetExtension(model.Image.FileName);
                var path = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", filename + extension);
                string directory = Path.GetDirectoryName(path);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                FileStream fs = new FileStream(path, FileMode.Create);
                await model.Image.CopyToAsync(fs);

                model.PassportImageUrl = Path.GetFileName(path);
            }
            return model.PassportImageUrl;
        }

        [HttpPost]
        public JsonResult Capture(string Image)
        {
            return Json("Saved");
        }

        [HttpPost]
        public async Task<ActionResult> Create(VisualAssessmentResultViewModel model, string token)
        {
            ViewBag.Token = token;
            try
            {
                model.PassOrFail = model.PassOrFailInt == null? null: (PassOrFail)model.PassOrFailInt;


                var visualAcuitys = _visualAcuityScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var visualFieldScores = _visualFieldScoreRepositoryQuery.FilterAsync(x => x.IsActive).Result.ToList();
                var colourVisionScores = _visualAssessmentResultRepository.GetColorVisionScores();
                var countries = _locationService.GetCountries();

                ViewBag.Countries = countries;
                ViewBag.VisualAcuity = new SelectList(visualAcuitys, "Score", "Score");
                ViewBag.VisualFieldScores = new SelectList(visualFieldScores, "Score", "Score");
                ViewBag.ColourVisionScores = new SelectList(colourVisionScores, "Id", "Value");
                ViewBag.SingleImage = new SelectList(visualAcuitys.Where(x => x.Id > 4), "Score", "Score");
                ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

                if (model.Action == Status.Complete)
                {
                    if (!ValidateModelState(model))
                    {
                        return View(model);
                    }
                }

                if (model.Image != null)
                {
                    bool isValidSize = Utility.ValidatePassport(model.Image);
                    if (!isValidSize)
                    {
                        model.Errors.Add("Your passport photo is too large. Kindly upload a photo that is 120KB or less");
                        return View(model);
                    }
                }

                if (string.IsNullOrEmpty(model.IdentityNumber))
                {
                    ModelState.AddModelError("IdentityNumber", "Please enter National ID or Passport Number");
                    model.Errors.Add("Please enter National ID or Passport Number");
                    return View(model);
                }

                model.PassportImageUrl = await SaveImage(model);

                var optometristUser = _optometristFirmUserRepositoryQuery.FilterAsync(x => x.ApplicationUserId == _authUser.UserId).Result.FirstOrDefault();

                if (optometristUser == null)
                {
                    model.Errors.Add("Sorry! You have not been mapped to an Optometrist Firm");
                    return View(model);
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

                Slot slot = _slotRepositoryQuery.FilterAsync(x => x.OptometristFirmId == optometristUser.OptometristFirmId && x.AccessType == (model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory)).Result.FirstOrDefault();
                if (model.Action == Status.Complete)
                {
                    if (slot == null)
                    {
                        model.Errors.Add("There is no available slot to continue with this assessment result");
                        return View(model);
                    }
                    if (slot.Quantity <= 0)
                    {
                        model.Errors.Add("There is no available slot to continue with this assessment result");
                        return View(model);
                    }
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

                        //_visualAssessmentResultRepositoryQuery.FilterAsync(x => x.Id == model.Id).Result.FirstOrDefault();
                        if (model.Id == 0)
                        {

                            string referenceNumber = _visualAssessmentResultRepository.GenerateReferenceNo(optometristUser.OptometristFirmId, (Status)model.Status);

                            if (model.Status == Status.Complete)
                            {
                                if (string.IsNullOrEmpty(model.PassportImageUrl) && model.Image == null)
                                {
                                    await scope.RollbackAsync();
                                    ModelState.AddModelError("PassportImageUrl", "Please capture/upload passport");
                                    model.Errors.Add("Please capture/upload passport");
                                    return View(model);
                                }
                            }

                            

                            if (model.ResultServiceType == ResultServiceType.LearnerDriversLicence) model.TestType = TestType.NewTest;
                            else model.TestType = TestType.ReTest;

                            VisualAssessmentResult visualAssessmentResult = new VisualAssessmentResult()
                            {
                                //NameTitle = model.NameTitle,
                                PassOrFail = (model.ResultConclusion == "Fit to drive" || model.ResultConclusion == "Fit to drive with glasses") ? PassOrFail.Pass : PassOrFail.Fail,
                                PassResult = model.PassResult,
                                Surname = model.Surname,
                                Gender = model.Gender,
                                //DriversLicence = model.DriversLicence,
                                //DVLAReferenceNo = model.DVLAReferenceNo,
                                FirstName = model.FirstName,
                                OtherName = model.OtherName,
                                DOB = (DateTime?)model.DOB,
                                PostalAddress = model.PostalAddress,
                                ContactNumber = model.ContactNumber,
                                Nationality = model.Nationality,
                                Email = string.IsNullOrEmpty(model.Email) ? "" : _emailService.IsValidEmail(model.Email.Trim()) ? model.Email.Trim() : "",
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
                                PathologicalRemarks = model.PathologicalRemarks,
                                ResultConclusion = model.ResultConclusion,
                                ResultServiceType = model.ResultServiceType,
                                OptometristFirmId = optometristUser.OptometristFirmId,
                                ReferenceNumber = referenceNumber,
                                CreatedBy = _authUser.UserId,
                                IsActive = true,
                                IsDeleted = false,
                                TestDate = DateTime.UtcNow,
                                PassportImageUrl = model.PassportImageUrl,
                                Status = model.Status,
                                IsSynchronized = false,
                                TestType = (TestType)model.TestType,
                                //OldDVLAReferenceNo = model.OldDVLAReferenceNo,
                                IsTransmitted = false,
                                CreatedDate = DateTime.UtcNow,
                                IsRegistration = false,
                                ContrastSensitivity_BCV = model.ContrastSensitivity_BCV,
                                AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory,
                                PassportNumber = model.IdentityType == IdentityType.InternationalPassport ? model.IdentityNumber : null,
                                NationalID = model.IdentityType == IdentityType.NationalIDCard ? model.IdentityNumber : null,
                                DvlaLicenseNumber = model.DvlaLicenseNumber
                            };
                            context.VisualAssessmentResults.Add(visualAssessmentResult);
                            await context.SaveChangesAsync();
                            if (model.Status == Status.Complete)
                            {

                                slot.Quantity = slot.Quantity - 1;
                                await context.SaveChangesAsync();

                                string result = model.PassOrFail.ToString();

                                _smsRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, referenceNumber, result, context);
                                //send email
                                _notificationRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, referenceNumber, result, model.Email, context);
                            }
                            await scope.CommitAsync();
                            TempData["SuccessMessage"] = "Eye Test Result created successfully";
                            _AuditRepo.AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Eye Test Result");

                            if (model.Status == Status.Complete)
                            {
                                return RedirectToAction("Detail", new { page = "Details", token = Utility.Encrypt(visualAssessmentResult.Id.ToString()) });
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
                        else
                        {


                            var visualAssessmentResult = await context.VisualAssessmentResults.FirstOrDefaultAsync(x => x.Id == model.Id);
                            if (visualAssessmentResult == null)
                            {
                                model.Errors.Add("Invalid Assessment Result");
                                return View(model);
                            }
                            string passFile = model.PassportImageUrl;
                            if (model.Status == Status.Complete)
                            {
                                if (string.IsNullOrEmpty(visualAssessmentResult.PassportImageUrl) && string.IsNullOrEmpty(model.PassportImageUrl) && model.Image == null)
                                {
                                    await scope.RollbackAsync();
                                    ModelState.AddModelError("PassportImageUrl", "Please capture/upload passport");
                                    model.Errors.Add("Please capture/upload passport");
                                    return View(model);
                                }
                                if (model.DOB == null)
                                {
                                    await scope.RollbackAsync();
                                    ModelState.AddModelError("DOB", "Select Date of Birth");
                                    model.Errors.Add("Select Date of Birth");
                                    return View(model);
                                }
                            }

                            if (model.ResultServiceType == ResultServiceType.LearnerDriversLicence) model.TestType = TestType.NewTest;
                            else model.TestType = TestType.ReTest;

                            //visualAssessmentResult.NameTitle = model.NameTitle;
                            visualAssessmentResult.Gender = model.Gender;
                            visualAssessmentResult.PassOrFail = model.PassOrFail;
                            visualAssessmentResult.PassResult = model.PassOrFail == PassOrFail.Fail ? null : model.PassResult;
                            visualAssessmentResult.Surname = model.Surname;
                            visualAssessmentResult.ContrastSensitivity_BCV = model.ContrastSensitivity_BCV;
                            //visualAssessmentResult.DriversLicence = model.DriversLicence;
                            //visualAssessmentResult.DVLAReferenceNo = model.DVLAReferenceNo;
                            visualAssessmentResult.FirstName = model.FirstName;
                            visualAssessmentResult.OtherName = model.OtherName;
                            visualAssessmentResult.DOB = (DateTime?)model.DOB;
                            visualAssessmentResult.PostalAddress = model.PostalAddress;
                            visualAssessmentResult.ContactNumber = model.ContactNumber;
                            visualAssessmentResult.Nationality = model.Nationality;
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
                            visualAssessmentResult.PassportImageUrl = string.IsNullOrEmpty(visualAssessmentResult.PassportImageUrl) ? model.PassportImageUrl : visualAssessmentResult.PassportImageUrl;
                            visualAssessmentResult.Status = model.Status;
                            visualAssessmentResult.TestType = (TestType)model.TestType;
                            visualAssessmentResult.TestDate = DateTime.UtcNow;
                            //visualAssessmentResult.OldDVLAReferenceNo = model.OldDVLAReferenceNo;
                            visualAssessmentResult.AccessType = model.ResultServiceType == ResultServiceType.LearnerDriversLicence ? AccessType.LearnerDriversLicence : AccessType.OtherLicenceCategory;
                            visualAssessmentResult.IsTransmitted = false;
                            visualAssessmentResult.ReferenceNumber = string.IsNullOrEmpty(visualAssessmentResult.ReferenceNumber) ? _visualAssessmentResultRepository.GenerateReferenceNo(optometristUser.OptometristFirmId, (Status)model.Status) : visualAssessmentResult.ReferenceNumber;
                            visualAssessmentResult.CreatedBy = _authUser.UserId;
                            visualAssessmentResult.ModifiedBy = _authUser.UserId;
                            visualAssessmentResult.ModifiedDate = DateTime.UtcNow;

                            //visualAssessmentResult.TestDate = DateTime.UtcNow;
                            await context.SaveChangesAsync();

                            //if (!string.IsNullOrEmpty(passFile) && passFile.Contains(".png"))
                            //{
                            //    var deleteFilePath = Path.Combine(_environment.ContentRootPath, "wwwroot", "Passports", passFile);
                            //    System.IO.File.Delete(deleteFilePath);
                            //}
                            //Send Sms Notification
                            if (model.Status == Status.Complete)
                            {
                                slot.Quantity = slot.Quantity - 1;
                                string result = model.PassOrFail.ToString(); //EnumHelper<PassOrFail>.GetDisplayValue(model.PassOrFail.GetValueOrDefault());

                                _smsRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, visualAssessmentResult.ReferenceNumber, result, context);
                                //send email
                                _notificationRepository.SendAssessmentResult(model.FirstName, model.ContactNumber, visualAssessmentResult.ReferenceNumber, result, model.Email, context);

                            }
                            await scope.CommitAsync();
                            TempData["SuccessMessage"] = "Record saved successfully";
                            _AuditRepo.AddAudit(Activities.CREATE_VISUAL_ASSESSMENT_RESULT, "Create Visual Assessment Result");

                            if (model.Status == Status.Complete)
                            {
                                return RedirectToAction("Detail", new { page = "Details", token = Utility.Encrypt(visualAssessmentResult.Id.ToString()) });
                            }
                            else
                            {
                                return RedirectToAction("Index");
                            }
                        }
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

                        if (result.CreatedBy == "System")
                        {
                            var optometristFirmUser = await _context.OptometristFirmUsers.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x => x.OptometristFirmId == result.OptometristFirmId);
                            optometrist = optometristFirmUser.ApplicationUser;
                        }

                        ViewBag.Optometrist = optometrist.LastName + " " + optometrist.FirstName;

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
                            //CreatedByFullName = optometrist.FirstName + " " + optometrist.LastName,
                            //CreatedByUsername = optometrist.UserName,
                            DateCreated = result.CreatedDate,
                            DigitalAddress = result.OptometristFirm.DigitalAddress,
                            DistrictName = result.OptometristFirm.District?.Name,
                            DOB = result.DOB,
                            //DriversLicence = result.DriversLicence,
                            //DVLAReferenceNo = result.DVLAReferenceNo,
                            Email = result.Email,
                            FirstName = result.FirstName,
                            //FormNumber = result.FormNumber,
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
                            MobileNumber = result.ContactNumber,
                            //NameTitle = result.NameTitle,
                            //Optometrist = optometrist.FullName,
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
                            TaxIdentificationNumber = result.Nationality,
                            TelephoneNumber = result.ContactNumber,
                            TestDate = result.TestDate,
                            Unaided_OD = result.Unaided_OD,
                            Unaided_OS = result.Unaided_OS,
                            Unaided_OU = result.Unaided_OU,
                            UpdatedBy = result.ModifiedBy,
                            UserName = optometrist.UserName, 
                            
                            //UpdatedByUsername = optometrist.UserName
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


        [HttpGet]
        public IActionResult InitiateEyeTest()
        {
            ApplicantViewModel model = new();
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