using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Infrastructure.Database.Context;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DVLA.VerificationPortal.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IUserRepository, UserRepository>();


            
            return services;
        }
    }
}
