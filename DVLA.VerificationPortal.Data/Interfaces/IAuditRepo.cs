using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Shared.DTOs;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IAuditRepo
    {
        Task AddAuditAsync(string action, string description);

        Task<List<ActivityModel>> GetAuditAsync(AuditFilterModel model);

    }
}
