using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.DashboardModule
{
    public interface IAnalyticRepository
    {
        DashboardViewModel GetDashboardData(int? optometristFirmId = null);
        ChartSummaryCount GetAvailableSlots(long? optometristFirmId = null);

        ChartSummaryCount GetUsedSlots(long? optometristFirmId = null);


        ChartSummaryCount GetApprovedRequestCount(long? optometristFirmId = null);

        ChartSummaryCount GetDeclinedRequestCount(long? optometristFirmId = null);

        List<ChartCount> GetSychronizationChartCount(long? optometristFirmId = null);

        List<ChartCount> GetUsedSlotChartCount(long? optometristFirmId = null);

        List<ChartCount> GetRequestChartCount(long status, long? optometristFirmId = null);

        List<ChartCount> GetIncomeChartCount();

        List<ChartCount> GetOptometristFirmChartCount();
    }
}
