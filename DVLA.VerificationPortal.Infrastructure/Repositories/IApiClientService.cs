using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure.Repositories
{
    public interface IApiClientService
    {
        Task<ApiClient?> AuthenticateAsync(string secret);
        Task AuditLogAsync(string controller, string action, string? apiKey);
        string? ApiKey { get; }
    }
}
