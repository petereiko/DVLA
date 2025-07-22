using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DVLA.VerificationPortal.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ISearchResultService, SearchResultService>();
            services.AddTransient<IApiClientService, ApiClientService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAuthUser, AuthUser>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IAuditRepo, AuditRepo>();
            services.AddTransient<IOptometristFirmSynchronization, OptometristFirmSynchronization>();
            return services;
        }
    }
}
