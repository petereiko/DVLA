using DVLA.Data.Models.Auth;
using DVLA.DATA.Domains;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace DVLA.API.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(ApplicationUser user, IEnumerable<string> roles, OptometristFirmUser optometristFirmUser, out DateTime expiresAtUtc)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiresMinutes = jwtSection.GetValue("ExpiresMinutes", 60);

            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            {
                throw new InvalidOperationException("Jwt:Key must be configured and at least 32 characters long.");
            }

            var roleList = roles?.ToList() ?? new List<string>();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                new Claim("FullName", $"{user.LastName} {user.FirstName}".Trim()),
                new Claim("Roles", string.Join(",", roleList)),
                new Claim("OptometristFirmId", optometristFirmUser?.OptometristFirmId.ToString() ?? "0")
            };

            claims.AddRange(roleList.Select(role => new Claim(ClaimTypes.Role, role)));

            expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
