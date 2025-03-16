using DVLA.Data.Models.DataObjects.DTOs;
using DVLA.Data.Models.DataObjects.UtilityObjects;
using DVLA.Data.Models.DataObjects.ViewModels;
using DVLA.Data.Models.Enumerables;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.VisualAssessmentResultModule
{
    public interface IVisualAssessmentResultRepository
    {
        PaginationResponseModel<List<VisualAssessmentResultItemViewModel>> FetchAssessmentResults(PaginationRequestModel<ClientSearchRequest> model);


        string GenerateReferenceNo(int optometristFirmId);
        string GenerateFormNo();
        VisualAssessmentResultModel FetchAssessmentResult(string ReferenceNumber);
        List<VisualAssessmentResultModel> FetchAssessmentResults(long? optometristAdminId, long? optometristId, long? id);
        ResultViewModel FetchAssessmentResults(int displayLength, int displayStart, int sortCol, string sortDir, string search, Int64? optometricId);
        ResultViewModel FetchAssessmentResults(ClientSearchRequest model);
        List<VisualAssessmentResultModel> FetchAssessmentResultsAdmin(Int64? optometricId);
        List<ColorVisionScoresModel> GetColorVisionScores();
        PaginationResponseModel<List<VisualAssessmentResultListItem>> GetVisualAssessmentResult(PaginationRequestModel pagination, int? optometristFirmId, Status? status, DateTime? startDate, DateTime? endDate, string DSReference);

        List<SelectListItem> ResultConclusion();


        Task<MessageResponse> Transmit(VisualAssessmentTransmissionModel model);
        Task<MessageResponse> LogBulkTransmission(List<VisualAssessmentTransmissionModel> data);
        Task<MessageResponse<List<string>>> TransmitBulk(List<VisualAssessmentTransmissionModel> data);
    }
}
