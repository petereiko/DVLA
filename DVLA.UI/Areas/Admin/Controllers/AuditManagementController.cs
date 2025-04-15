using DVLA.Business.LocationModule;
using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{AppRoles.SYSTEMADMIN},{AppRoles.FACILITYOWNER},{AppRoles.OPTOMETRIST}")]
    public class AuditManagementController : Controller
    {
        private readonly IAuditRepo _auditRepo;
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IRepositoryQuery<Module> _moduleRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly ILogger<AuditManagementController> _logger;
        private IVisualAssessmentResultRepository _visualAssessmentResultRepository;
        private readonly ILocationService _locationService;

        public AuditManagementController(IAuditRepo AuditRepo, IUserRepository userRepository, IRepositoryQuery<Module> moduleRepositoryQuery, IRepositoryQuery<OptometristFirm> optometristFirmQuery, IReportRepository reportRepository, ILogger<AuditManagementController> logger, IVisualAssessmentResultRepository visualAssessmentResultRepository, ILocationService locationService)
        {
            _auditRepo = AuditRepo;
            _userRepository = userRepository;
            _reportRepository = reportRepository;
            _optometristFirmQuery = optometristFirmQuery;
            _moduleRepositoryQuery = moduleRepositoryQuery;
            _logger = logger;
            _visualAssessmentResultRepository = visualAssessmentResultRepository;
            _locationService = locationService;
        }

        // GET: Admin/AuditManagement
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");

            var countries = _locationService.GetCountries();
            ViewBag.Countries = countries;
            AuditGridViewModel model = new();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(AuditGridViewModel model)
        {
            try
            {
                model.Items = await _auditRepo.GetAuditAsync(model.Filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            ViewBag.ResultConclusions = new SelectList(_visualAssessmentResultRepository.ResultConclusion(), "Value", "Text");
            var countries = _locationService.GetCountries();
            ViewBag.Countries = countries;
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> ExportAudit(AuditGridViewModel model)
        {
            if (model != null)
            {
                model.ExportItems = await _auditRepo.GetAuditExportAsync(model.Filter);

                string fileName = "Audit_Report.xlsx";
                var json = JsonConvert.SerializeObject(model.ExportItems);
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