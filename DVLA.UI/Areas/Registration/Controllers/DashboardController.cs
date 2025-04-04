using DVLA.Business.DashboardModule;
using DVLA.Business.Repository;
using DVLA.Business.SlotModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace DVLA.UI.Areas.Registration.Controllers
{
    [Area("Registration")]
    [Authorize(Roles = AppRoles.FRONTOFFICER)]
    public class DashboardController : Controller
    {
        private readonly IAuditRepo _AuditRepo;
        private ISlotRepository _slotRepository;
        private IAnalyticRepository _analyticRepository;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        private readonly IRepositoryQuery<OptometristFirm> _optometristFirmQuery;
        private readonly IRepositoryQuery<Slot> _slotRepositoryQuery;
        private readonly IAuthUser _authUser;

        private static readonly Random rand = new Random();
        private string GetRandomColor()
        {
            return string.Format("rgba({0},{1},{2},{1})", rand.Next(80, 256), rand.Next(80, 256), rand.Next(80, 256));
        }

        public DashboardController(IAuditRepo AuditRepo, IAnalyticRepository analyticRepository, ISlotRepository slotRepository,
            IRepositoryQuery<OptometristFirm> optometristFirmQuery, IRepositoryQuery<OptometristFirmUser> optometristUserQuery,
            IRepositoryQuery<Slot> slotRepositoryQuery, IUserService userService, IAuthUser authUser)
        {
            _AuditRepo = AuditRepo;
            _slotRepository = slotRepository;
            _analyticRepository = analyticRepository;
            _optometristUserQuery = optometristUserQuery;
            _optometristFirmQuery = optometristFirmQuery;
            _slotRepositoryQuery = slotRepositoryQuery;
            _authUser = authUser;
        }

        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetAvailableSlotCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetAvailableSlots(optometristUser.OptometristFirmId);

            return Json(new { success = true, result });
        }

        public JsonResult GetUsedSlotCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetUsedSlots(optometristUser.OptometristFirmId);

            return Json(new { success = true, result });
        }

        public JsonResult GetApprovedApplicationCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetApprovedRequestCount(optometristUser.OptometristFirmId);

            return Json(new { success = true, result });
        }

        public JsonResult GetDeclinedApplicationCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetDeclinedRequestCount(optometristUser.OptometristFirmId);

            return Json(new { success = true, result });
        }


        public ContentResult GetSynchronizationChartCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetSychronizationChartCount(optometristUser.OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ContentResult GetUsedSlotChartCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetUsedSlotChartCount(optometristUser.OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ContentResult GetApprovedApplicationChartCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Approved, optometristUser.OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ContentResult GetDeclinedApplicationChartCount()
        {
            var optometristUser = _optometristUserQuery.Filter(u => u.ApplicationUserId == _authUser.UserId).FirstOrDefault();
            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Reject, optometristUser.OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}