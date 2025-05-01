using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface ISearchResultService
    {
        Task<IEnumerable<VisualAssessmentResultDto>> GetResultsAsync(string searchTerm);
        Task<TestResultDto> GetResultAsync(string? reference);
        Task<VisualAssessmentResultDto> GetResultAsync(int id);
        Task<MessageResponse> PushBulk(VisualAssessmentResultDto result);
        Task<MessageResponse> Push(VisualAssessmentResultDto model);
        Task<MessageResponse> VerifyResult(string token);
        Task<MessageResponse<string>> VerifyResultByReferenceAsync(string referenceNumber);
    }
}
