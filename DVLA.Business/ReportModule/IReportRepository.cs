using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.DATA.Domains;
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
        Task<List<OptometristFirmModel>> FetchAllOptometristFirms(int? regionId, int? district);
        Task<TransmissionGridDto> FetchDataAsync(TransmissionGridDto model);
        List<VisualAssessmentResultDto> FetchAllPendingTransmissions();
        List<UpdateDocRequestDto> FetchAllPendingAuthDocUpdate();
        //Task<MessageResponse> PushDataAsync();
        Task<MessageResponse> PushDataAsync(long? id, string sourceConnString, string destConnString);
        byte[] WriteToExcel(string extension, DataTable dt);
    }
}
