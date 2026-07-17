using DVLA.API.Services;
using DVLA.Business.ApplicantModule;
using DVLA.Business.DashboardModule;
using DVLA.Business.EmailModule;
using DVLA.Business.LocationModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.OptometristFirmModule;
using DVLA.Business.PaymentModule;
using DVLA.Business.ReportModule;
using DVLA.Business.Repository;
using DVLA.Business.SlotModule;
using DVLA.Business.TempPasswordModule;
using DVLA.Business.UserModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Data;
using DVLA.Data.Models.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;

try
{
    var builder = WebApplication.CreateBuilder(args);

    var cultureInfo = new CultureInfo("en-GB");
    CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
    CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "DVLA API",
            Version = "v1"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter a valid JWT bearer token."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new List<string>()
            }
        });
    });

    builder.Services.AddDbContext<DVLADbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddMemoryCache();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
    builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

    builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1);
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 4;
    })
        .AddEntityFrameworkStores<DVLADbContext>()
        .AddUserManager<UserManager<ApplicationUser>>()
        .AddRoleManager<RoleManager<ApplicationRole>>()
        .AddUserStore<UserStore<ApplicationUser, ApplicationRole, DVLADbContext, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
        .AddRoleStore<RoleStore<ApplicationRole, DVLADbContext, string, ApplicationUserRole, IdentityRoleClaim<string>>>()
        .AddDefaultTokenProviders();

    var jwtSection = builder.Configuration.GetSection("Jwt");
    var jwtKey = jwtSection["Key"];
    if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    {
        throw new InvalidOperationException("Jwt:Key must be configured and at least 32 characters long.");
    }

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSection["Issuer"],
                ValidAudience = jwtSection["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });

    builder.Services.AddAuthorization();
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        options.OnRejected = async (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.ContentType = "application/json";

            await context.HttpContext.Response.WriteAsync(
                "{\"success\":false,\"message\":\"Too many requests. Please try again later.\"}",
                cancellationToken);
        };

        options.AddPolicy("Auth", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                GetClientPartitionKey(httpContext),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

        options.AddPolicy("AuthenticatedRead", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                GetClientPartitionKey(httpContext),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 120,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

        options.AddPolicy("SensitiveWrite", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                GetClientPartitionKey(httpContext),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 30,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

        options.AddPolicy("ExternalOperation", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                GetClientPartitionKey(httpContext),
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));
    });

    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppConstants"));
    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));

    builder.Services.AddTransient<IEmailService, EmailService>();
    builder.Services.AddTransient<IUserService, UserService>();
    builder.Services.AddTransient<IApplicantService, ApplicantService>();
    builder.Services.AddTransient<IVisualAssessmentResultRepository, VisualAssessmentResultService>();
    builder.Services.AddTransient<ILocationService, LocationService>();
    builder.Services.AddTransient<IOptometristService, OptometristService>();
    builder.Services.AddTransient<ISlotRepository, SlotRepository>();
    builder.Services.AddTransient<ISlotUsageRepository, SlotUsageRepository>();
    builder.Services.AddTransient<IReportRepository, ReportService>();
    builder.Services.AddScoped(typeof(IRepositoryQuery<>), typeof(GenericRepository<>));
    builder.Services.AddTransient<IAuditRepo, AuditRepo>();
    builder.Services.AddTransient<IUserRepository, UserRepository>();
    builder.Services.AddTransient<IAnalyticRepository, AnalyticRepository>();
    builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
    builder.Services.AddTransient<IPaymentService, PaymentService>();
    builder.Services.AddTransient<ISmsRepository, SmsRepository>();
    builder.Services.AddTransient<ITempPasswordService, TempPasswordService>();
    builder.Services.AddTransient<IAuthUser, AuthUser>();
    builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "DVLA API v1");
        options.RoutePrefix = "swagger";
    });

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseRateLimiter();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.Error.WriteLine("DVLA.API failed to start.");
    Console.Error.WriteLine(ex);
    Debug.WriteLine(ex);
    throw;
}

static string GetClientPartitionKey(HttpContext httpContext)
{
    var userId = httpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    if (!string.IsNullOrWhiteSpace(userId))
    {
        return $"user:{userId}";
    }

    var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].ToString();
    if (!string.IsNullOrWhiteSpace(forwardedFor))
    {
        return $"ip:{forwardedFor.Split(',')[0].Trim()}";
    }

    return $"ip:{httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? IPAddress.None.ToString()}";
}
