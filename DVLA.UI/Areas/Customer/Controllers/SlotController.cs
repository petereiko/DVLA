
using DVLA.Business.OptometristFirmModule;
using DVLA.Business.PaymentModule;
using DVLA.Business.Repository;
using DVLA.Business.SlotModule;
using DVLA.Business.UserModule;
using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.PaystackDtos;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using DVLA.DATA.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;


namespace DVLA.UI.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]

    public class SlotController : Controller
    {
        private ISlotRepository _slotRepository;
        private readonly ISlotUsageRepository _slotUsageRepository;
        private readonly IOptometristService _optometristRepository;
        private readonly IAuditRepo _auditRepo;
        private readonly string currentUserId;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        private readonly IRepositoryQuery<OptometristFirmUser> _optometristUserQuery;
        
        public SlotController(ISlotRepository slotRepository, ISlotUsageRepository slotUsageRepository,
            IOptometristService optometristRepository, IAuditRepo auditRepo, IUserService userService, IPaymentService paymentService, IRepositoryQuery<OptometristFirmUser> optometristUserQuery)
        {
            _slotRepository = slotRepository;
            _slotUsageRepository = slotUsageRepository;
            _auditRepo = auditRepo;
            _optometristRepository = optometristRepository;
            currentUserId = userService.GetUserData().Id;
            _userService = userService;
            _paymentService = paymentService;
            _optometristUserQuery = optometristUserQuery;
        }

        [HttpGet]
        public ActionResult MyRequests()
        {
            IEnumerable<SlotRequestModel> slotRequests = _slotRepository.FetchCustomerSlotRequests(currentUserId);
            _auditRepo.AddAudit(Activities.VIEW_SLOT_REQUEST, "View Slot Requests");
            return View(slotRequests);
        }
        [Authorize(Roles =$"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}" )]
        [HttpGet]
        public IActionResult InitiateSlotRequest()
        {
            SlotRequestViewModel model = new ();
            model.FormData = new SlotRequestModel { SlotPriceList = _slotRepository.AmountPerSlot() };
            return View(model);
        }
        [Authorize(Roles = $"{AppRoles.FACILITYOWNER}, {AppRoles.OPTOMETRIST}")]
        [HttpPost]
        public ActionResult InitiateSlotRequest(SlotRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Errors.AddRange(ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                model.FormData = new SlotRequestModel { SlotPriceList = _slotRepository.AmountPerSlot() };
                return View(model);
            }
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            model.FormData.OptometristFirmId = OptometristFirmId;
            model.FormData.Status = SlotRequestStatus.Pending;
            model.FormData.PaymentMethod = PaymentMethod.Online;
            MessageResponse response = _slotRepository.CreateSlotRequest(model.FormData);
            if (response.Success)
            {
                TempData["SuccessMessage"] = response.Message;
                _auditRepo.AddAudit(Activities.INITIATE_SLOT_REQUEST, "Initiate Slot Request");
                return RedirectToAction("MyRequests", "Slot");
            }
            else
            {
                model.FormData = new SlotRequestModel { SlotPriceList = _slotRepository.AmountPerSlot() };
                model.Errors.Add(response.Message);
                return View(model);
            }

        }


        [HttpPost]
        public async Task<JsonResult> ProceedToPayment([FromBody]InitiatePaymentRequest model)
        {
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            model.OptometristFirmId = OptometristFirmId;
            model.UserId = currentUserId;
            var userData = _userService.GetUserData();
            model.email = userData.Email;
            return Json(await _paymentService.InitiatePayment(model));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmPayment(string trxref)//trxref=wwbrw1ah86&reference=wwbrw1ah86
        {
            var result = _paymentService.VerifyPayment(trxref);
            if (result.status)
            {
                TempData["SuccessMessage"] = "Your payment has been successfully recieved and slots allocated accordingly.";
                return RedirectToAction("MyRequests");
            }
            TempData["ErrorMessage"] = result.message;
            return RedirectToAction("InitiateSlotRequest");
        }

        

        [HttpGet]
        public ActionResult SlotReorder()
        {
            var optometristUser = _optometristUserQuery.Filter(x => x.ApplicationUserId == currentUserId).FirstOrDefault();
            int OptometristFirmId = optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            List<SlotModel> slot = _slotRepository.FetchSlotReOrderLevelByOptometristfirm(OptometristFirmId).ToList();                       
            return View(slot);
        }

        [HttpGet]
        public ActionResult UpdateSlotReOrder(string token)
        {
            long id = long.Parse(Utility.Decrypt(token));

            SlotModel slot = _slotRepository.FetchSlotReOrderLevel(id);
            return View(slot);
        }

        [HttpPost]
        public ActionResult UpdateSlotReOrder(SlotModel model)
        {
            if(model.ReorderLevel < 1)
            {
                ModelState.AddModelError("ReorderLevel", "Reorder level must be greater than 1");
                return View(model);
            }

            MessageResponse response = _slotRepository.UpdateSlotReOrderLevel(model);
            if (response.Success)
            {
                _auditRepo.AddAudit(Activities.UPDATE_SLOT_REORDER,"Updated Slot Reorder Level");
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("SlotReorder");
            }
            else
            {
                ModelState.AddModelError("ReorderLevel", response.Message);
                _auditRepo.AddAudit(Activities.CREATE_SLOT_REORDER, "Failed to created Slot Reorder Level");
                return View("SlotReorder");
            }


        }

        [HttpGet]
        public JsonResult ComputeSlotQuantity(decimal amount, int accessType) 
        {
            MessageResponse response = new();
            try
            {
                response = _slotRepository.ComputeSlotQuantity(amount, (AccessType)accessType);
                var obj = new { status = true, message = "ok", quantity =Convert.ToInt32(response.Message) };
                return Json(obj);
            }
            catch(Exception ex) 
            {
                return Json(response);
            }
        }

        [HttpGet]
        public IActionResult SlotUsageStatistics()
        {
            return View(new SlotUsageViewModel());
        }

        [HttpPost]
        public ActionResult SlotUsageStatistics(SlotUsageViewModel model)
        {
            var temp = model.EndDate;
            int? OptometristFirmId = _userService.GetUserData().OptometristFirmId; //optometristUser == null ? 0 : optometristUser.OptometristFirmId;
            model.EndDate = model.EndDate.HasValue ? model.EndDate.Value.AddHours(23).AddMinutes(59).AddSeconds(59) : DateTime.Now.AddHours(23).AddMinutes(59).AddSeconds(59);
            IEnumerable<SlotUsageModel> slotUsages = _slotUsageRepository.FetchOptometristSlotUsage(model.StartDate, model.EndDate, model.AccessType, OptometristFirmId);
            model.SlotUsages = slotUsages;
            model.EndDate = temp;
            return View(model);
        }
    }
}