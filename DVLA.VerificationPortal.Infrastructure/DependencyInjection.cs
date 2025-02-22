using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DVLA.VerificationPortal.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}
