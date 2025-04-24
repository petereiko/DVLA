using DVLA.VerificationPortal.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IApiClientService
    {
        Task<ApiClient?> AuthenticateAsync(string secret);
        Task AuditLogAsync(string controller, string action);
        string? ApiName { get; }
        string? ApiKey { get; }
        int? ApiId { get; }
    }
}
