using System;
using System.Collections.Generic;
using System.Linq;
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

        public ApplicationUserDto? GetCachedUserData()
        {
            var httpContext = _contextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            var id = httpContext.Request.Cookies["Id"];
            var email = httpContext.Request.Cookies["Email"];
            var role = httpContext.Request.Cookies["Role"];

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(role))
            {
                return new ApplicationUserDto
                {
                    Id = id,
                    Email = email,
                    Role = role
                };
            }

            return null;
        }
    }
}
