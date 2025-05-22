using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public string Email  => _contextAccessor.HttpContext.Request.Cookies["Email"];
        public string UserId => _contextAccessor.HttpContext.Request.Cookies["Id"];
        public string Role => _contextAccessor.HttpContext.Request.Cookies["Role"];

        
    }
}
