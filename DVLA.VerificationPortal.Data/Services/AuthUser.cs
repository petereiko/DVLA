using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Shared.Constants;
using DVLA.VerificationPortal.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DVLA.VerificationPortal.Application.Services
{
    public class AuthUser:IAuthUser
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthUser(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string Email  => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value!;
        public string UserId => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        public string Role => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value!;
        public string UserName => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value!;
        public string CentreName => _contextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Country)?.Value!;
    }
}
