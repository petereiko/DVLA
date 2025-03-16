using DVLA.Data;
using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.PaystackDtos;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.SlotModule
{
    public interface ISlotRepository
    {
        MessageResponse<long> CreateSlot(SlotModel model);
        MessageResponse CreateSlotRequest(SlotRequestModel model);
        
        IEnumerable<OptometristFirmModel> GetOptometristFirms();
        MessageResponse SlotDeduction(SlotDeductionModel model);
        MessageResponse<long> CreateSlotPrice(SlotPriceModel model);
        SlotPriceModel GetSlotPrice(int id);
        SlotPriceModel GetSlotPrice();
        IEnumerable<SlotPriceModel> GetSlotPrices();
        MessageResponse UpdateSlotPrice(SlotPriceModel model, int id);
        OptometristFirmModel GetOptometristFirmByApplicationUserID(string applicationUserID);
        Task<IEnumerable<SlotRequestModel>> FetchSlotRequests(SlotRequestParameter request);
        Task<SlotRequestModel> FetchSlotRequestById(int id, int status);
        MessageResponse ApproveSlotRequest(int id);
        MessageResponse RejectSlotRequest(RejectSlotRequestModel model);
        MessageResponse Preview(int id);
        IEnumerable<SlotRequestModel> FetchCustomerSlotRequests(string applicationUserId);
        Task<int> FetchLowQuantitySlots(string applicationUserId);
        MessageResponse UpdateSlotReOrderLevel(SlotModel model);
        IEnumerable<SlotModel> FetchSlotReOrderLevels();
        SlotModel FetchSlotReOrderLevel(long id);
        IEnumerable<SlotModel> FetchSlotReOrderLevelByOptometristfirm(int optometristfirmId);
        MessageResponse ComputeSlotQuantity(decimal amountPaid, AccessType accessType);
        List<PriceModel> AmountPerSlot();

        Task<List<SlotRequestModel>> FetchSlotsForIncomeReport(DateTime? StartDate, DateTime? EndDate);
    }
}
