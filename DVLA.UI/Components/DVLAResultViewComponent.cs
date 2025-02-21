using DVLA.Business.Repository;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.DATA.Domains;
using DVLA.Data.Models.DataObjects.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DVLA.UI.Components
{
    public class DVLAResultViewComponent : ViewComponent
    {
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultRepositoryQuery;
        private readonly IRepositoryQuery<VisualAcuityScore> _visualAcuityScoreRepositoryQuery;
        private readonly IRepositoryQuery<VisualFieldScore> _visualFieldScoreRepositoryQuery;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly ILogger<DVLAResultViewComponent> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly DVLADbContext _context;
        private readonly IConfiguration _configuration;

        public DVLAResultViewComponent(IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultRepositoryQuery,
            IVisualAssessmentResultRepository visualAssessmentResultRepository,
            IRepositoryQuery<VisualAcuityScore> visualAcuityScoreRepositoryQuery,
            IRepositoryQuery<VisualFieldScore> visualFieldScoreRepositoryQuery, ILogger<DVLAResultViewComponent> logger, IWebHostEnvironment environment, DVLADbContext context, IConfiguration configuration)
        {
            _visualAcuityScoreRepositoryQuery = visualAcuityScoreRepositoryQuery;
            _visualFieldScoreRepositoryQuery = visualFieldScoreRepositoryQuery;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _visualAssessmentResultRepositoryQuery = visualAssessmentResultRepositoryQuery;
            _logger = logger;
            _environment = environment;
            _context = context;
            _configuration = configuration;
        }


        public async Task<IViewComponentResult> InvokeAsync(string token)
        {
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
                        //else
                        //{
                        //    model.PassportImageUrl = $"{_configuration["AppConstants:BaseUrl"]}/Passports/{model.PassportImageUrl}";
                        //}
                    }
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return View(model);
        }
    }
}
