using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Globalization;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Database.Context;
using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Infrastructure.MappingProfiles;
using DVLA.VerificationPortal.Infrastructure;
using DVLA.VerificationPortal.Application;
using DVLA.VerificationPortal.Middleware;
using DVLA.VerificationPortal.Shared.MappingProfiles;
using DVLA.VerificationPortal;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-GB"); // or whatever culture is correct
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddMemoryCache();
builder.Services.AddSignalR();



builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
    options.SlidingExpiration = true;
    options.Cookie.Name = "AuthDemo.Cookie";
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

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




builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session duration
    options.Cookie.HttpOnly = false; // Make the cookie HTTP only
    options.Cookie.IsEssential = true; // Make the cookie essential
});
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();

builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();


builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(InfrastructureMappingProfile));
    config.AddMaps(typeof(SharedMappingProfile));
});




var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");




//Create a scope to resolve scoped services
using (var scope = app.Services.CreateScope())
{
    var scopedProvider = scope.ServiceProvider;

    // Resolve the scoped service
    var userService = scopedProvider.GetRequiredService<IUserService>();

    // Call the method on the scoped service
    await userService.SeedRolesAsync();
    await userService.SeedSuperAdminAsync();
}

app.Run();
