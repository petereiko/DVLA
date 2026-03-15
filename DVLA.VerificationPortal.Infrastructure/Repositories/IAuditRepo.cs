using DVLA.VerificationPortal.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public interface IAuditRepo
    {
        Task<List<ActivityModel>> GetAuditAsync(AuditFilterModel filter);
        Task AddAuditAsync(string actionName, string description);
    }
}
