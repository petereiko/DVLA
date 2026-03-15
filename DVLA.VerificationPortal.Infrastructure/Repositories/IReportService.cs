using DVLA.VerificationPortal.Shared.DTOs;
using DVLA.VerificationPortal.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public interface IReportService
    {
        Task<IEnumerable<TestResultCountDto>> GetResults(DateTime StartDate, DateTime EndDate, PassOrFail? passOrFail);
        Task<IEnumerable<VerifiedItemDto>> GetVerifiedResults(DateTime StartDate, DateTime EndDate);
        Task<IEnumerable<TestResultDto>> VerifiedResultsByUser(string userId);
    }
}
