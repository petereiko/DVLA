
using DVLA.VerificationPortal.Infrastructure;
using DVLA.VerificationPortal.Infrastructure.Database.Context;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using AtlasWallet.Api.Security;
using DVLA.VerificationPortal.API.Security;

namespace DVLA.VerificationPortal.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var cultureInfo = new CultureInfo("en-GB"); // or whatever culture is correct
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.
            // Add Identity
            builder.Services.AddScoped<UserManager<ApplicationUser>>();
            builder.Services.AddScoped<RoleManager<ApplicationRole>>();
            builder.Services.AddScoped<SignInManager<ApplicationUser>>();

            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                //Configure Identity Options
                options.SignIn.RequireConfirmedAccount = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1);
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<ApplicationRole>>()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, string, ApplicationUserRole, IdentityRoleClaim<string>>>()
            .AddDefaultTokenProviders();

            builder.Services.AddInfrastructureServices();

            


            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            builder.Services.AddTransient<ApiKeyAuthorizationFilter>();
            builder.Services.AddTransient<IApiKeyValidator, ApiKeyValidator>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);
            //builder.Services.AddResponseCaching();
            builder.Services.AddCors(policyBuilder =>
                policyBuilder.AddDefaultPolicy(policy =>
                    policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
