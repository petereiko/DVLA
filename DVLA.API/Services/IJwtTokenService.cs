using DVLA.Data.Models.Auth;
using DVLA.DATA.Domains;
using System;
using System.Collections.Generic;

namespace DVLA.API.Services
{
    public interface IJwtTokenService
    {
        string CreateToken(ApplicationUser user, IEnumerable<string> roles, OptometristFirmUser optometristFirmUser, out DateTime expiresAtUtc);
    }
}
