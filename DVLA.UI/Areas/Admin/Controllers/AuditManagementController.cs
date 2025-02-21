using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = AppRoles.SYSTEMADMIN)]
    public class AuditManagementController : Controller
    {
        private readonly IAuditRepo _auditRepo;
        private readonly IUserRepository _userRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IRepositoryQuery<Module> _moduleRepositoryQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly ILogger<AuditManagementController> _logger;

        public AuditManagementController(IAuditRepo AuditRepo, IUserRepository userRepository, IRepositoryQuery<Module> moduleRepositoryQuery, IRepositoryQuery<OptometristFirm> optometristFirmQuery, IReportRepository reportRepository, ILogger<AuditManagementController> logger)
        {
            _auditRepo = AuditRepo;
            _userRepository = userRepository;
            _reportRepository = reportRepository;
            _optometristFirmQuery = optometristFirmQuery;
            _moduleRepositoryQuery = moduleRepositoryQuery;
            _logger = logger;
        }

        // GET: Admin/AuditManagement
        [HttpGet]
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Account");
            }

            ViewBag.OptometristFirms = _optometristFirmQuery.GetAll().ToList();
            ViewBag.Modules = _moduleRepositoryQuery.GetAll().ToList();
            ViewBag.Users = _userRepository.GetUsers(string.Empty, null);
            return View(new List<ActivityModel>());
        }

        [HttpPost]
        public async Task<ActionResult> Index(AuditFilterModel model)
        {
            if (string.IsNullOrEmpty(User.Identity.Name))
            {
                return RedirectToAction("Index", "Account");
            }

            var result = new List<ActivityModel>();
            try
            {
                result = await _auditRepo.GetAudit(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            ViewBag.OptometristFirms = _optometristFirmQuery.GetAllAsync().Result.ToList();
            ViewBag.Modules = _moduleRepositoryQuery.GetAllAsync().Result.ToList();
            ViewBag.Users = _userRepository.GetUsers(string.Empty, null);
            return View(result);
        }


        [HttpPost]
        public ActionResult ExportAudit(List<ActivityModel> model)
        {
            if (model != null)
            {
                string fileName = "Audit_Report.xlsx";
                var json = JsonConvert.SerializeObject(model);
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