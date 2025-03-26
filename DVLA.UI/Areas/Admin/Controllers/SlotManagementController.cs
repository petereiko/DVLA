using DVLA.Business.SlotModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DVLA.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{AppRoles.SYSTEMADMIN},{AppRoles.SLOTMANAGER},{AppRoles.FINANCE},{AppRoles.FACILITYOWNER}")]
    public class SlotManagementController : Controller
    {
        private readonly IAuditRepo _auditRepo;
        private ISlotRepository _slotRepository;
        private ISlotUsageRepository _slotUsageRepository;
        private ILogger<SlotManagementController> _logger;
        private readonly string currentUserId;
        private readonly IUserService _userService;
        public SlotManagementController(ISlotRepository slotRepository, IUserService userService, ISlotUsageRepository slotUsageRepository, IAuditRepo auditRepo, ILogger<SlotManagementController> logger)
        {
            _slotRepository = slotRepository;
            _slotUsageRepository = slotUsageRepository;
            _auditRepo = auditRepo;
            _logger = logger;
            _userService = userService;
            currentUserId = userService.GetUserData().Id;
        }
        // GET: Admin/SlotManagement
        public ActionResult SlotRequest(int? status=1)
        {
            //IEnumerable<SlotRequestModel> slots = await _slotRepository.FetchSlotRequests((SlotRequestStatus)status.Value, 50);
            AdminSlotRequestViewModel model = new()
            {
                //Slots = slots,
                parameter = new() { length = 50, status = 1 }
            };
            _auditRepo.AddAudit(Activities.VIEW_SLOT_REQUEST, "View Slot Request");
            return View(model);
        }

        [HttpPost]
        public async Task<PartialViewResult> GetSlotItems(SlotRequestParameter model)
        {
            IEnumerable<SlotRequestModel> slots = await _slotRepository.FetchSlotRequests(model);
            return PartialView("~/Views/Partials/_SlotItems.cshtml", slots);
        }


        [HttpPost]
        public async Task<IActionResult> SlotRequest(SlotRequestParameter parameter, int? status)
        {
            if (!status.HasValue) return View("SlotRequest");
            IEnumerable<SlotRequestModel> slotRequests = parameter == null ? await _slotRepository.FetchSlotRequests(parameter)
                : await _slotRepository.FetchSlotRequests(parameter);
            parameter = parameter == null ? new SlotRequestParameter { status = status.Value } : new SlotRequestParameter { status = status.Value, length = parameter.length };
            AdminSlotRequestViewModel model = new AdminSlotRequestViewModel
            {
                parameter = parameter,
                Slots = slotRequests.ToList()
            };
            _auditRepo.AddAudit(Activities.VIEW_SLOT_REQUEST,"Viewed Slot Requests");
            return View(model);
        }


        public ActionResult SlotDeduction()
        {
            SlotDeductionViewModel model = new SlotDeductionViewModel
            {
                OptometristFirms = _slotRepository.GetOptometristFirms().Select(x => new SelectListItem
                {
                    Text = x.BusinessName,
                    Value = x.Id.ToString()
                })
            };
            _auditRepo.AddAudit(Activities.VIEW_SLOT_REDUCTION, "Viewed Slot Deduction");
            return View(model);
        }

        [HttpPost]
        public ActionResult SlotDeduction([FromBody]SlotDeductionModel model)
        {
            MessageResponse response = _slotRepository.SlotDeduction(model);
            return Json(response);
        }


        public ActionResult SlotPrices()
        {
            IEnumerable<SlotPriceModel> slotPrices = _slotRepository.GetSlotPrices();
            _auditRepo.AddAudit(Activities.VIEW_SLOT_PRICE,"Viewed Slot Prices");
            return View(slotPrices);
        }

        public ActionResult CreateSlotPrice()
        {
            _auditRepo.AddAudit(Activities.CREATE_SLOT_PRICE,"Viewed Create Slot Price");
            CreateSlotPriceViewModel model = new CreateSlotPriceViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateSlotPrice(CreateSlotPriceViewModel model)
        {
            if (model.FormData.AccessType == 0)
            {
                ModelState.AddModelError("AccessType", "Please select Access Type");
            }
            if (!ModelState.IsValid)
            {
                foreach (var modelStates in ModelState.Values)
                {
                    foreach (var error in modelStates.Errors)
                    {
                        model.Errors.Add(error.ErrorMessage);
                    }
                }

                return View(model);
            }
            model.FormData.CreatedBy = currentUserId;
            model.FormData.CreatedDate = DateTime.Now;
            var response = _slotRepository.CreateSlotPrice(model.FormData);
            if (response.Success)
            {
                _auditRepo.AddAudit(Activities.CREATE_SLOT_PRICE,"Created " + model.FormData.Price + " Slot Price");
                TempData["SuccessMessage"] =  response.Message;
                _auditRepo.AddAudit(Activities.CREATE_SLOT_PRICE, "Create Slot Price");
                return RedirectToAction("SlotPrices", "SlotManagement");
            }
            else
            {
                _auditRepo.AddAudit(Activities.CREATE_SLOT_PRICE,"Failed to created " + model.FormData.Price + " Slot Price");
                model.Errors.Add(response.Message);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult EditSlotPrice(int id)
        {
            SlotPriceModel price = _slotRepository.GetSlotPrice(id);
            CreateSlotPriceViewModel model = new CreateSlotPriceViewModel { FormData = price };
            _auditRepo.AddAudit(Activities.VIEW_SLOT_PRICE,"Viewed Edit Slot Price");
            return View(model);
        }

        [HttpPost]
        public ActionResult EditSlotPrice(CreateSlotPriceViewModel model, int id)
        {
            if (model.FormData.AccessType == 0)
            {
                ModelState.AddModelError("AccessType", "Please select Access Type");
            }
            if (!ModelState.IsValid)
            {
                foreach (var modelStates in ModelState.Values)
                {
                    foreach (var error in modelStates.Errors)
                    {
                        model.Errors.Add(error.ErrorMessage);
                    }
                }
                return View(model);
            }
            model.FormData.UpdatedBy = currentUserId;
            model.FormData.ModifiedDate = DateTime.Now;
            var response = _slotRepository.UpdateSlotPrice(model.FormData, id);
            if (response.Success)
            {
                _auditRepo.AddAudit(Activities.EDIT_SLOT_PRICE,"Edit Slot Price to " + model.FormData.Price);
                TempData["SuccessMessage"] = response.Message;
                _auditRepo.AddAudit(Activities.UPDATE_SLOT_PRICE, "Update Slot Price");
                return RedirectToAction("SlotPrices", "SlotManagement");
            }
            else
            {
                _auditRepo.AddAudit(Activities.EDIT_SLOT_PRICE,"Failed to Edit Slot Price");
                model.Errors.Add(response.Message);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<JsonResult> Approve(int id)
        {
            var response = _slotRepository.ApproveSlotRequest(id);
            if (response.Success)
            {
                var slot = await _slotRepository.FetchSlotRequestById(id,3);
                _auditRepo.AddAudit(Activities.APPROVE_SLOT_REQUEST,$"Approved {slot.Quantity} {slot.AccessType.ToString()} Slots for ::: {slot.BusinessName} ::: Request Id:{slot.Id}");
            }
            return Json(response);
        }

        [HttpPost]
        public JsonResult Reject([FromBody]RejectSlotRequestModel model)
        {
            MessageResponse response = _slotRepository.RejectSlotRequest(model);
            if (response.Success)
            {
                _auditRepo.AddAudit(Activities.REJECT_SLOT_REQUEST, "Reject Slot Request");
            }
            return Json(response);
        }

        [HttpGet]
        public JsonResult Preview(int id)
        {
            var response = _slotRepository.Preview(id);
            _auditRepo.AddAudit(Activities.PREVIEW_SLOT_REQUEST,"Slot Preview");
            return Json(response);
        }

        

        //public ActionResult SlotUsageStatistics() 
        //{
        //    IEnumerable<ChartModel> daily = _slotUsageRepository.SlotUsageByTestCenterByDay();
        //    IEnumerable<ChartModel> weekly = _slotUsageRepository.SlotUsageByTestCenterByWeek();
        //    IEnumerable<ChartModel> monthly = _slotUsageRepository.SlotUsageByTestCenterByMonth();
        //    IEnumerable<ChartModel> yearly = _slotUsageRepository.SlotUsageByTestCenterByYear();
        //    SlotUsageStatisticsViewModel model = new SlotUsageStatisticsViewModel
        //    {
        //        PerDay = daily,
        //        PerWeek = weekly,
        //        PerMonth = monthly,
        //        PerYear = yearly
        //    };
        //    SlotUsageViewModel vmodel = new();
           
        //    return View(model);
        //}

        public IActionResult IncomeReport()
        {
            return View(new IncomeReportViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> IncomeReport(IncomeReportViewModel model) 
        {
            //if (!ModelState.IsValid)
            //{
            //    foreach (var modelStates in ModelState.Values)
            //    {
            //        foreach (var error in modelStates.Errors)
            //        {
            //            model.Errors.Add(error.ErrorMessage);
            //        }
            //    }
            //    return View(model);
            //}
            //model.StartDate = model.StartDate.HasValue ? model.StartDate.Value : DateTime.Now;
            //model.EndDate = model.EndDate.HasValue ? model.EndDate.Value : DateTime.Now;
            model.SlotRequests = await _slotRepository.FetchSlotsForIncomeReport(model.StartDate, model.EndDate);
            return View(model);
        }

        [HttpGet]
        public ActionResult SlotUsageStatistics()
        {
            SlotUsageViewModel model = new();
            //var temp = model.EndDate;
            //model.EndDate = model.EndDate.HasValue ? model.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : DateTime.Now.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            //IEnumerable<SlotUsageModel> slotUsages = _slotUsageRepository.FetchSlotUsage(model.StartDate, model.EndDate, model.AccessType);
            //model.SlotUsages = slotUsages;
            //model.EndDate = temp;
            return View(model);
        }

        [HttpPost]
        public ActionResult SlotUsageStatistics(SlotUsageViewModel model) 
        {
            int? optometristFirmId = _userService.GetUserData().OptometristFirmId;

            IEnumerable<SlotUsageModel> slotUsages = _slotUsageRepository.FetchOptometristSlotUsage(model.StartDate, model.EndDate, model.AccessType, optometristFirmId);
            model.SlotUsages = slotUsages;

            model.TotalAccessPurchase = slotUsages.Sum(x => x.TotalSlotPurchased);
            model.TotalBalance = slotUsages.Sum(x => x.Balance);
            model.TotalAccessUsed = slotUsages.Sum(x => x.TotalSlotUsed);


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SlotPurchased()
        { 
            SlotStatisticsViewModel model = new()
            {
                SlotStatisticsFilter = new() { AccessType = null },
                OptometristFirmId = _userService.GetUserData().OptometristFirmId
            };
            model = await _slotUsageRepository.SlotPurchasedAsync(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SlotPurchased(SlotStatisticsViewModel model)
        {
            model.OptometristFirmId = _userService.GetUserData().OptometristFirmId;
            model = await _slotUsageRepository.SlotPurchasedAsync(model);
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> SlotUsed()
        {
            SlotStatisticsViewModel model = new()
            {
                SlotStatisticsFilter = new()
                {
                    AccessType = null
                },
                OptometristFirmId = _userService.GetUserData().OptometristFirmId
            };
            model = await _slotUsageRepository.SlotUsedAsync(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SlotUsed(SlotStatisticsViewModel model)
        {
            model.OptometristFirmId = _userService.GetUserData().OptometristFirmId;
            model = await _slotUsageRepository.SlotUsedAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SlotBalance()
        {
            SlotStatisticsViewModel model = new()
            {
                SlotStatisticsFilter = new() { AccessType = null },
                OptometristFirmId = _userService.GetUserData().OptometristFirmId
            };
            model = await _slotUsageRepository.SlotBalanceAsync(model);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SlotBalance(SlotStatisticsViewModel model)
        {
            model.OptometristFirmId = _userService.GetUserData().OptometristFirmId;
            model = await _slotUsageRepository.SlotBalanceAsync(model);
            return View(model);
        }
    }
}