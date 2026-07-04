using DVLA.Business.DashboardModule;
using DVLA.Business.Repository;
using DVLA.Business.SlotModule;
using DVLA.Business.UserModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System;
using System.Linq;

namespace DVLA.API.Controllers
{
    [Authorize]
    [EnableRateLimiting("AuthenticatedRead")]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private static readonly Random Rand = new Random();
        private readonly IAnalyticRepository _analyticRepository;
        private readonly ISlotUsageRepository _slotUsageRepository;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IRepositoryQuery<VisualAssessmentResult> _visualAssessmentResultQuery;
        private readonly IRepositoryQuery<Slot> _slotQuery;
        private readonly IAuthUser _authUser;

        public DashboardController(
            IAnalyticRepository analyticRepository,
            ISlotUsageRepository slotUsageRepository,
            IRepositoryQuery<OptometristFirmUser> optometristUserQuery,
            IRepositoryQuery<VisualAssessmentResult> visualAssessmentResultQuery,
            IRepositoryQuery<Slot> slotQuery,
            IAuthUser authUser)
        {
            _analyticRepository = analyticRepository;
            _slotUsageRepository = slotUsageRepository;
            _optometristUserQuery = optometristUserQuery;
            _visualAssessmentResultQuery = visualAssessmentResultQuery;
            _slotQuery = slotQuery;
            _authUser = authUser;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var optometristFirmId = _authUser.OptometristFirmId;
            return Ok(new
            {
                SlotUsage = BuildSlotUsageBar(optometristFirmId),
                CurrentUser = new
                {
                    _authUser.UserId,
                    _authUser.Email,
                    _authUser.FullName,
                    _authUser.Roles,
                    OptometristFirmId = optometristFirmId
                }
            });
        }

        [HttpGet("data")]
        public IActionResult GetDashboardData()
        {
            var optometristFirmId = _authUser.OptometristFirmId;
            var bar = _slotUsageRepository.FetchSlotUsageBar(optometristFirmId);
            return Ok(new DashboardViewModel
            {
                OtherGrantedSlotCount = bar.OtherUsedSlot + bar.OtherUnusedSlot,
                LearnerGrantedSlotCount = bar.LearnerUsedSlot + bar.LearnUnusedSlot,
                LearnerUtilizedSlotCount = bar.LearnerUsedSlot,
                OtherUtilizedSlotCount = bar.OtherUsedSlot
            });
        }

        [HttpGet("available-slot-count")]
        public IActionResult GetAvailableSlotCount([FromQuery] int? optometristFirmId = null)
        {
            return Ok(new { success = true, result = _analyticRepository.GetAvailableSlots(optometristFirmId) });
        }

        [HttpGet("used-slot-count")]
        public IActionResult GetUsedSlotCount([FromQuery] int? optometristFirmId = null)
        {
            return Ok(new { success = true, result = _analyticRepository.GetUsedSlots(optometristFirmId) });
        }

        [HttpGet("approved-application-count")]
        public IActionResult GetApprovedApplicationCount([FromQuery] int? optometristFirmId = null)
        {
            return Ok(new { success = true, result = _analyticRepository.GetApprovedRequestCount(optometristFirmId) });
        }

        [HttpGet("declined-application-count")]
        public IActionResult GetDeclinedApplicationCount([FromQuery] int? optometristFirmId = null)
        {
            return Ok(new { success = true, result = _analyticRepository.GetDeclinedRequestCount(optometristFirmId) });
        }

        [HttpGet("synchronization-chart-count")]
        public IActionResult GetSynchronizationChartCount([FromQuery] int? optometristFirmId = null)
        {
            var result = _analyticRepository.GetSychronizationChartCount(optometristFirmId);
            result.ForEach(x => x.Color = GetRandomColor());
            return Ok(result);
        }

        [HttpGet("used-slot-chart-count")]
        public IActionResult GetUsedSlotChartCount([FromQuery] int? optometristFirmId = null)
        {
            var result = _analyticRepository.GetUsedSlotChartCount(optometristFirmId);
            result.ForEach(x => x.Color = GetRandomColor());
            return Ok(result);
        }

        [HttpGet("approved-application-chart-count")]
        public IActionResult GetApprovedApplicationChartCount([FromQuery] int? optometristFirmId = null)
        {
            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Approved, optometristFirmId);
            result.ForEach(x => x.Color = GetRandomColor());
            return Ok(result);
        }

        [HttpGet("declined-application-chart-count")]
        public IActionResult GetDeclinedApplicationChartCount([FromQuery] int? optometristFirmId = null)
        {
            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Reject, optometristFirmId);
            result.ForEach(x => x.Color = GetRandomColor());
            return Ok(result);
        }

        [HttpGet("customer")]
        public IActionResult CustomerIndex()
        {
            var optometristUser = _optometristUserQuery
                .FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm)
                .FirstOrDefault();

            return Ok(new
            {
                CompanyName = optometristUser?.OptometristFirm?.BusinessName ?? string.Empty,
                OptometristFirmId = optometristUser?.OptometristFirmId,
                AvailableQuantity = GetAvailableQuantity(optometristUser)
            });
        }

        private SlotUsageBarModel BuildSlotUsageBar(int? optometristFirmId)
        {
            return new SlotUsageBarModel
            {
                LearnerUsedSlot = _visualAssessmentResultQuery.Filter(x => x.Status == Status.Complete && x.ResultServiceType == ResultServiceType.LearnerDriversLicence && (optometristFirmId == null || x.OptometristFirmId == optometristFirmId)).Count(),
                LearnUnusedSlot = _slotQuery.Filter(x => x.AccessType == AccessType.LearnerDriversLicence && (optometristFirmId == null || x.OptometristFirmId == optometristFirmId)).Sum(x => x.Quantity),
                OtherUsedSlot = _visualAssessmentResultQuery.Filter(x => x.Status == Status.Complete && x.ResultServiceType != ResultServiceType.LearnerDriversLicence && (optometristFirmId == null || x.OptometristFirmId == optometristFirmId)).Count(),
                OtherUnusedSlot = _slotQuery.Filter(x => x.AccessType != AccessType.LearnerDriversLicence && (optometristFirmId == null || x.OptometristFirmId == optometristFirmId)).Sum(x => x.Quantity)
            };
        }

        private int? GetAvailableQuantity(OptometristFirmUser optometristUser)
        {
            if (optometristUser == null)
            {
                return null;
            }

            return _slotQuery.Filter(x => x.OptometristFirmId == optometristUser.OptometristFirmId)
                .FirstOrDefault()
                ?.Quantity ?? 0;
        }

        private static string GetRandomColor()
        {
            return string.Format("rgba({0},{1},{2},{1})", Rand.Next(80, 256), Rand.Next(80, 256), Rand.Next(80, 256));
        }
    }
}
