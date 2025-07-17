using AutoMapper;
using DVLA.VerificationPortal.Domain.Interfaces;
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
        private readonly IMapper _mapper;
        public CustomClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor,
        IMapper mapper)
        : base(userManager, roleManager, optionsAccessor)
        {
            _userManager = userManager;
            _mapper = mapper;
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
            ApplicationUserDto model = _mapper.Map<ApplicationUserDto>(user);
            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            model.Role = userRoles.FirstOrDefault();
            return model;
        }
    }
}
