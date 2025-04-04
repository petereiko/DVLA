using DVLA.Business.DashboardModule;
using DVLA.Business.Repository;
using DVLA.Business.SlotModule;
using DVLA.Business.UserModule;
using DVLA.DATA.Domains;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DVLA.UI.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private readonly IAnalyticRepository _analyticRepository;
        private ISlotUsageRepository _slotUsageRepository;
        private readonly IUserService _userService;
        private readonly IRepositoryQuery<OptometristFirm> _optometristRepository;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserRepository;
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultQuery;
        private readonly IRepositoryQuery<Slot> _slotQuery;
        private readonly IAuthUser _authUser;


        private static readonly Random rand = new Random();
        private string GetRandomColor()
        {
            return string.Format("rgba({0},{1},{2},{1})", rand.Next(80, 256), rand.Next(80, 256), rand.Next(80, 256));
        }

        public DashboardController(IAnalyticRepository analyticRepository, ISlotUsageRepository slotUsageRepository, IAuditRepo auditRepo, IUserService userService, IRepositoryQuery<OptometristFirm> optometristRepository, IRepositoryQuery<OptometristFirmUser> optometristUserRepository, IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultQuery, IRepositoryQuery<Slot> slotQuery, IAuthUser authUser)
        {
            _AuditRepo = auditRepo;
            _analyticRepository = analyticRepository;
            _slotUsageRepository = slotUsageRepository;
            _userService = userService;
            _optometristRepository = optometristRepository;
            _optometristUserRepository = optometristUserRepository;
            _visualAssessmentResultQuery = visualAssessmentResultQuery;
            _slotQuery = slotQuery;
            _authUser = authUser;
        }

        public ActionResult Index()
        {
            bool isSysAdmin = User.IsInRole(AppRoles.SYSTEMADMIN);
            bool isFinanceOfficer = User.IsInRole(AppRoles.FINANCE);

            SlotUsageBarModel model = new SlotUsageBarModel();
            if (isSysAdmin || isFinanceOfficer)
            {
                model = new SlotUsageBarModel
                {
                    LearnerUsedSlot = _visualAssessmentResultQuery.Filter(x => x.Status == Status.Complete && x.ResultServiceType == ResultServiceType.LearnerDriversLicence).Count(),
                    LearnUnusedSlot = _slotQuery.Filter(x => x.AccessType == AccessType.LearnerDriversLicence).Sum(x => x.Quantity),
                    OtherUsedSlot = _visualAssessmentResultQuery.Filter(x => x.Status == Status.Complete && x.ResultServiceType != ResultServiceType.LearnerDriversLicence).Count(),
                    OtherUnusedSlot = _slotQuery.Filter(x => x.AccessType != AccessType.LearnerDriversLicence).Sum(x => x.Quantity)
                };
            }
            else
            {
                int? optometristFirmId = _authUser.OptometristFirmId;
                model = new SlotUsageBarModel
                {
                    LearnerUsedSlot = _visualAssessmentResultQuery.Filter(x => x.Status == Status.Complete && x.ResultServiceType == ResultServiceType.LearnerDriversLicence && x.OptometristFirmId ==optometristFirmId).Count(),
                    LearnUnusedSlot = _slotQuery.Filter(x => x.AccessType == AccessType.LearnerDriversLicence && x.OptometristFirmId == optometristFirmId).Sum(x => x.Quantity),
                    OtherUsedSlot = _visualAssessmentResultQuery.Filter(x => x.Status == Status.Complete && x.ResultServiceType != ResultServiceType.LearnerDriversLicence && x.OptometristFirmId == optometristFirmId).Count(),
                    OtherUnusedSlot = _slotQuery.Filter(x => x.AccessType != AccessType.LearnerDriversLicence && x.OptometristFirmId == optometristFirmId).Sum(x => x.Quantity)
                };
            }

            //SlotUsageBarModel model = isSysAdmin ? _slotUsageRepository.FetchSlotUsageBar(null) : _slotUsageRepository.FetchSlotUsageBar(_userService.GetUserData().OptometristFirmId);
            return View(model);
        }

        [HttpGet]
        public JsonResult GetDashboardData()
        {
            DashboardViewModel result = new();
            bool isSysAdmin = User.IsInRole(AppRoles.SYSTEMADMIN);
            SlotUsageBarModel bar = isSysAdmin ? _slotUsageRepository.FetchSlotUsageBar(null) : _slotUsageRepository.FetchSlotUsageBar(_authUser.OptometristFirmId);
            result.OtherGrantedSlotCount = bar.OtherUsedSlot + bar.OtherUnusedSlot;
            result.LearnerGrantedSlotCount = bar.LearnerUsedSlot + bar.LearnUnusedSlot;
            result.LearnerUtilizedSlotCount = bar.LearnerUsedSlot;
            result.OtherUtilizedSlotCount = bar.OtherUsedSlot;
            return Json(result);
        }

        [HttpGet]
        public JsonResult GetAvailableSlotCount()
        {
            //var result = _analyticRepository.GetAvailableSlots(OptometristFirmId);
            var result = _analyticRepository.GetAvailableSlots(null);

            return Json(new { success = true, result });
        }

        [HttpGet]
        public JsonResult GetUsedSlotCount()
        {
            var result = _analyticRepository.GetUsedSlots();

            return Json(new { success = true, result });
        }

        [HttpGet]
        public JsonResult GetApprovedApplicationCount()
        {
            var result = _analyticRepository.GetApprovedRequestCount();

            return Json(new { success = true, result });
        }

        [HttpGet]
        public JsonResult GetDeclinedApplicationCount()
        {
            var result = _analyticRepository.GetDeclinedRequestCount();

            return Json(new { success = true, result });
        }

        [HttpGet]
        public ContentResult GetSynchronizationChartCount()
        {
            var result = _analyticRepository.GetSychronizationChartCount();
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [HttpGet]
        public ContentResult GetUsedSlotChartCount()
        {
            var result = _analyticRepository.GetUsedSlotChartCount();
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [HttpGet]
        public ContentResult GetApprovedApplicationChartCount()
        {
            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Approved);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [HttpGet]
        public ContentResult GetDeclinedApplicationChartCount()
        {
            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Reject);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [HttpGet]
        public ContentResult GetOptometristChartCount()
        {
            var result = _analyticRepository.GetOptometristFirmChartCount();
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }

        [HttpGet]
        public ContentResult GetIncomeChartCount()
        {
            var result = _analyticRepository.GetIncomeChartCount();
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}
