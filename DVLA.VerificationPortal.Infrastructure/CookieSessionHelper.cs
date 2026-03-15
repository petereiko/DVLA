using DVLA.VerificationPortal.Infrastructure.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Infrastructure
{
    public static class CookieSessionHelper
    {
        private const string SessionDataClaim = "session_data";

        // Pack your custom object into claims
        public static List<Claim> ToClaims(UserProperty userProp)
        {
            var json = JsonSerializer.Serialize(userProp);

            return new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userProp.Id.ToString()),
            new Claim(ClaimTypes.Name,           userProp.Username),
            new Claim(ClaimTypes.Email,          userProp.Email),
            new Claim(ClaimTypes.Role,           userProp.Role),
            new Claim(SessionDataClaim,          json)
        };
        }

        public static string? GetUserId(HttpContext context)
        {
            return context.User.Identity.GetUserId();
        }
    }
}
