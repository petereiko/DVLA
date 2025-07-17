using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<VerifiedItemDto>> GetVerifiedResults(DateTime StartDate, DateTime EndDate);

        Task<IEnumerable<TestResultCountDto>> GetResults(DateTime StartDate, DateTime EndDate, PassOrFail? passOrFail);
        Task<int> GetUsedSlot(int? optometristFirmId);

        Task<IEnumerable<TestResultDto>> VerifiedResultsByUser(string userId);
    }
}
