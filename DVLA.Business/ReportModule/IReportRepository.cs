using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.ReportModule
{
    public interface IReportRepository
    {
        Task<List<CustomerReportViewModel>> GetCustomerSynchronizationReport(SynchronizationReportFilterViewModel model);
        Task<List<SynchronizationReportViewModel>> GetSynchronizationReport(SynchronizationReportFilterViewModel model);
        List<ClientSearchModel> FetchClientSearchOld(ClientSearchParameter searchParameter, Int64? optometristFirmId = null);
        Task<List<ClientModel>> FetchClientSearch(ClientSearchParameter searchParameter, string optometristAdminId = null, string optometristId = null);
        Task<List<SlotReductionModel>> FetchSlotReductionLogs(SlotReductionLogSearchParameter search);
        Task<List<OptometristFirmModel>> FetchAllOptometristFirms();
        byte[] WriteToExcel(string extension, DataTable dt);
    }
}
