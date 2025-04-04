using DVLA.Business.DashboardModule;
using DVLA.Business.Repository;
using DVLA.Business.SlotModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using DVLA.UI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]
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
            IRepositoryQuery<Slot> slotRepositoryQuery, IAuditRepo auditRepo, IUserService userService, IAuthUser authUser)
        {
            _AuditRepo = AuditRepo;
            _slotRepository = slotRepository;
            _analyticRepository = analyticRepository;
            _optometristUserQuery = optometristUserQuery;
            _optometristFirmQuery = optometristFirmQuery;
            _slotRepositoryQuery = slotRepositoryQuery;
            _AuditRepo = auditRepo;
            _authUser = authUser;
        }

        public ActionResult Index()
        {
            //ViewBag.Controller = HttpContext.Request.RequestContext.RouteData.Values["controller"];
            //ViewBag.Action = HttpContext.Request.RequestContext.RouteData.Values["action"];
            //ViewBag.Id = HttpContext.Request.RequestContext.RouteData.Values["id"];



            //int totalQuantity = _slotRepository.FetchLowQuantitySlots(currentUserId);
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x=>x.OptometristFirm).FirstOrDefault();
            int optometristFirmId = 0;//optometristUser.OptometristFirmId;
            if (optometristUser != null) optometristFirmId = optometristUser.OptometristFirmId;
            ViewBag.CompanyName = optometristUser == null ? "" : optometristUser.OptometristFirm.BusinessName;
            if (User.IsInRole("Optometrist"))
            {
                var slot = _slotRepositoryQuery.FilterAsync(x => x.OptometristFirmId == optometristFirmId).Result.FirstOrDefault();
                var slotReOrderObject = _slotRepository.FetchSlotReOrderLevel(optometristFirmId);
                if (slot != null)
                {
                    if (optometristUser.OptometristFirm.ReorderLevel > slot.Quantity)
                    {
                        ViewBag.AvailableQuantity = slot.Quantity.ToString();
                    }
                }
                else 
                {
                    ViewBag.AvailableQuantity = "0";
                }
            }
            //AddAudit(Activities.VIEW_DASHBOARD, "View Customer Dashboard");

            return View();
        }


        public JsonResult GetAvailableSlotCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            var result = _analyticRepository.GetAvailableSlots(OptometristFirmId);

            return Json(new { success = true, result });
        }

        public JsonResult GetUsedSlotCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetUsedSlots(OptometristFirmId);

            return Json(new { success = true, result });
        }

        public JsonResult GetApprovedApplicationCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetApprovedRequestCount(OptometristFirmId);

            return Json(new { success = true, result });
        }

        public JsonResult GetDeclinedApplicationCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetDeclinedRequestCount(OptometristFirmId);

            return Json(new { success = true, result });
        }


        public ContentResult GetSynchronizationChartCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetSychronizationChartCount(OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ContentResult GetUsedSlotChartCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetUsedSlotChartCount(OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ContentResult GetApprovedApplicationChartCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Approved, OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }


        public ContentResult GetDeclinedApplicationChartCount()
        {
            var optometristUser = _optometristUserQuery.FilterInclude(x => x.ApplicationUserId == _authUser.UserId, x => x.OptometristFirm).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;

            var result = _analyticRepository.GetRequestChartCount((long)SlotRequestStatus.Reject, OptometristFirmId);
            foreach (ChartCount n in result)
            {
                n.Color = GetRandomColor();
            }
            return Content(JsonConvert.SerializeObject(result), "application/json");
        }
    }
}