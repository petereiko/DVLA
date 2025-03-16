using DVLA.Data;
using DVLA.Data.Models.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using DVLA.Business.BackgroundJobModule;
using DVLA.Business.Hubs;
using DVLA.Business.UserModule;
using DVLA.Business.EmailModule;
using DVLA.Business.LocationModule;
using DVLA.Business.OptometristFirmModule;
using DVLA.Business.SlotModule;
using DVLA.Business.ReportModule;
using DVLA.Business.ApplicantModule;
using DVLA.Business.VisualAssessmentResultModule;
using DVLA.Business.Repository;
using DVLA.Business.DashboardModule;
using DVLA.Business.NotificationModule;
using DVLA.Business.PaymentModule;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-GB"); // or whatever culture is correct
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DVLADbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

// Add Identity
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
    .AddEntityFrameworkStores<DVLADbContext>()
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, DVLADbContext, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>>()
.AddRoleStore<RoleStore<ApplicationRole, DVLADbContext, string, ApplicationUserRole, IdentityRoleClaim<string>>>()
.AddDefaultTokenProviders();

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.Cookie.HttpOnly = true;
//        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(10); // Set cookie expiration time
//        options.LoginPath = "/Account/Login";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.SlidingExpiration = true; // Enables sliding expiration
//    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    });


//Add UserManager<ApplicationUser>, RoleManager<ApplicationRole>, and SignInManager<ApplicationUser>
builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<ApplicationRole>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(25); // Set session duration
    options.Cookie.HttpOnly = false; // Make the cookie HTTP only
    options.Cookie.IsEssential = true; // Make the cookie essential
});
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

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
builder.Services.AddTransient<IAuditRepo, AuditRepo>();
builder.Services.AddTransient<BackgroundJobService>();


builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseDefaultTypeSerializer()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(15),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));


builder.Services.AddHangfireServer();



var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template")),
    RequestPath = "/wwwroot/Template"
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AppFile")),
    RequestPath = "/wwwroot/AppFile"
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logos")),
    RequestPath = "/wwwroot/logos"
});

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Passports")),
    RequestPath = "/passports"
});
app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notification");

RecurringJob.AddOrUpdate<BackgroundJobService>("SendBulkEmail", service => service.SendBulkEmail(), "*/2 * * * *");
RecurringJob.AddOrUpdate<BackgroundJobService>("VerifyTransfers", service => service.VerifyPayments(), "*/1 * * * *");
RecurringJob.AddOrUpdate<BackgroundJobService>("PushVisualAssessmentResult", service => service.PushVisualAssessmentResult(), "*/5 * * * *");//Every 1 minute


//Create a scope to resolve scoped services
using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;

    // Resolve the scoped service
    var userService = scopedProvider.GetRequiredService<IUserService>();

    // Call the method on the scoped service
    await userService.SeedRoles();
}

app.Run();
