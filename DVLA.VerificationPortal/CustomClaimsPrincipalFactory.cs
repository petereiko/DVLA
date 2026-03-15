using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DVLA.VerificationPortal
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor)
        {
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ApplicationUserDto model = await GetUserDependencies(user);
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName ?? ""));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id!));
            identity.AddClaim(new Claim(ClaimTypes.Role, model.Role ?? ""));
            identity.AddClaim(new Claim(ClaimTypes.Country, model.CentreName ?? ""));
            return identity;
        }

        private async Task<ApplicationUserDto> GetUserDependencies(ApplicationUser user)
        {
            ApplicationUserDto model = new()
            {
                CentreName = user.CentreName,
                CreatedDate = user.CreatedDate,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Id = user.Id,
                IsActive = user.IsActive,
                IsFirstLogin = user.IsFirstLogin,
                LastLoginDate = user.LastLoginDate,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName
            };
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            model.Role = userRoles.FirstOrDefault();
            return model;
        }
    }
}
