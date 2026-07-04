using DVLA.Business.PaymentModule;
using DVLA.Business.SlotModule;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.PaystackDtos;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DVLA.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly ISlotRepository _slotRepository;
        private readonly IPaymentService _paymentService;

        public SlotsController(ISlotRepository slotRepository, IPaymentService paymentService)
        {
            _slotRepository = slotRepository;
            _paymentService = paymentService;
        }

        [HttpGet("requests")]
        public async Task<IActionResult> GetRequests([FromQuery] SlotRequestParameter request)
        {
            request ??= new SlotRequestParameter();
            return Ok(await _slotRepository.FetchSlotRequests(request));
        }

        [HttpGet("requests/{id:int}")]
        public async Task<IActionResult> GetRequest(int id, [FromQuery] int status = 1)
        {
            return Ok(await _slotRepository.FetchSlotRequestById(id, status));
        }

        [HttpGet("customer-requests/{applicationUserId}")]
        public IActionResult GetCustomerRequests(string applicationUserId)
        {
            return Ok(_slotRepository.FetchCustomerSlotRequests(applicationUserId));
        }

        [HttpPost("requests")]
        public IActionResult CreateRequest([FromBody] SlotRequestModel model)
        {
            var result = _slotRepository.CreateSlotRequest(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("requests/{id:int}/approve")]
        public IActionResult Approve(int id)
        {
            var result = _slotRepository.ApproveSlotRequest(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("requests/reject")]
        public IActionResult Reject([FromBody] RejectSlotRequestModel model)
        {
            var result = _slotRepository.RejectSlotRequest(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("requests/{id:int}/preview")]
        public IActionResult Preview(int id)
        {
            var result = _slotRepository.Preview(id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("prices")]
        public IActionResult GetPrices()
        {
            return Ok(_slotRepository.GetSlotPrices());
        }

        [HttpGet("prices/{id:int}")]
        public IActionResult GetPrice(int id)
        {
            return Ok(_slotRepository.GetSlotPrice(id));
        }

        [HttpPost("prices")]
        public IActionResult CreatePrice([FromBody] SlotPriceModel model)
        {
            var result = _slotRepository.CreateSlotPrice(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("prices/{id:int}")]
        public IActionResult UpdatePrice(int id, [FromBody] SlotPriceModel model)
        {
            var result = _slotRepository.UpdateSlotPrice(model, id);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("deduction")]
        public IActionResult SlotDeduction([FromBody] SlotDeductionModel model)
        {
            var result = _slotRepository.SlotDeduction(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("reorder-levels")]
        public IActionResult GetReorderLevels()
        {
            return Ok(_slotRepository.FetchSlotReOrderLevels());
        }

        [HttpGet("reorder-levels/{id:long}")]
        public IActionResult GetReorderLevel(long id)
        {
            return Ok(_slotRepository.FetchSlotReOrderLevel(id));
        }

        [HttpPut("reorder-levels")]
        public IActionResult UpdateReorderLevel([FromBody] SlotModel model)
        {
            var result = _slotRepository.UpdateSlotReOrderLevel(model);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("compute-quantity")]
        public IActionResult ComputeSlotQuantity([FromQuery] decimal amount, [FromQuery] AccessType accessType)
        {
            var result = _slotRepository.ComputeSlotQuantity(amount, accessType);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("amount-per-slot")]
        public IActionResult AmountPerSlot()
        {
            return Ok(_slotRepository.AmountPerSlot());
        }

        [HttpGet("income-report")]
        public async Task<IActionResult> IncomeReport([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            return Ok(await _slotRepository.FetchSlotsForIncomeReport(startDate, endDate));
        }

        [HttpPost("payments/initiate")]
        public async Task<IActionResult> InitiatePayment([FromBody] InitiatePaymentRequest model)
        {
            return Ok(await _paymentService.InitiatePayment(model));
        }

        [HttpGet("payments/verify/{reference}")]
        public IActionResult VerifyPayment(string reference)
        {
            return Ok(_paymentService.VerifyPayment(reference));
        }
    }
}
