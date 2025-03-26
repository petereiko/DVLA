using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.SlotModule
{
    public interface ISlotUsageRepository
    {
        IEnumerable<ChartModel> SlotUsageByTestCenterByDay();
        IEnumerable<ChartModel> SlotUsageByTestCenterByWeek();
        IEnumerable<ChartModel> SlotUsageByTestCenterByMonth();
        IEnumerable<ChartModel> SlotUsageByTestCenterByYear();

        IEnumerable<ChartModel> SlotUsageByTestCenterByDay(long userId);
        IEnumerable<ChartModel> SlotUsageByTestCenterByWeek(long userId);
        IEnumerable<ChartModel> SlotUsageByTestCenterByMonth(long userId);
        IEnumerable<ChartModel> SlotUsageByTestCenterByYear(long userId);

        Task<long[]> GetTotalSlots();

        List<TestAnalysisModel> FetchTestAnalysis(long? optometristId, DateTime StartDate, DateTime EndDate);
        List<TestAnalysisModel> FetchWeeklySlots(long? optometristId);
        List<TestAnalysisModel> FetchMonthlySlots(long? optometristId);
        List<TestAnalysisModel> FetchYearlySlots(long? optometristId);

        SlotUsageBarModel FetchSlotUsageBar(int? optometristFirmId = null);

        List<SlotUsageModel> FetchSlotUsage(DateTime? StartDate, DateTime? EndDate, AccessType? accessType);
        List<SlotUsageModel> FetchOptometristSlotUsage(DateTime? StartDate, DateTime? EndDate, AccessType? accessType, int? optometrist = null);


        Task<SlotStatisticsViewModel> SlotPurchasedAsync(SlotStatisticsViewModel model);
        Task<SlotStatisticsViewModel> SlotUsedAsync(SlotStatisticsViewModel model);
        Task<SlotStatisticsViewModel> SlotBalanceAsync(SlotStatisticsViewModel model);
    }
}
