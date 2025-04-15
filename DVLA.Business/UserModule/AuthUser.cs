using DVLA.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.UserModule
{
    public class AuthUser: IAuthUser
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly DVLADbContext _context;

        public AuthUser(IHttpContextAccessor accessor, DVLADbContext context)
        {
            _accessor = accessor;
            _context = context;
        }

        public string Email => _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value!;
        public string UserId => _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        public string Roles => _accessor.HttpContext?.User?.FindFirst("Roles")?.Value!;
        public string FullName => _accessor.HttpContext?.User?.FindFirst("FullName")?.Value!;
        public int? OptometristFirmId
        {
            get
            {
                int? optoId = null;
                int optometristFirmId;
                string claim = _accessor.HttpContext?.User?.FindFirst("OptometristFirmId")?.Value!;
                bool result = int.TryParse(claim, out optometristFirmId);
                optoId = result ? optometristFirmId > 0 ? optometristFirmId : null : null;

                return optoId;
            }
        }

        public string OptometristFirmName
        {
            get
            {
                string name = "";
                int? optoId = null;
                int optometristFirmId;
                string claim = _accessor.HttpContext?.User?.FindFirst("OptometristFirmId")?.Value!;
                bool result = int.TryParse(claim, out optometristFirmId);
                optoId = result ? optometristFirmId > 0 ? optometristFirmId : null : null;
                if (optoId != null)
                {
                    var optometristFirm = _context.OptometristFirms.AsNoTracking().FirstOrDefault(x => x.Id == optoId);
                    name = optometristFirm?.BusinessName;
                }

                return name;
            }
        }

        public string BaseUrl
        {
            get
            {
                return $"{_accessor.HttpContext.Request.Scheme}://{_accessor.HttpContext.Request.Host}";
            }
        }
    }
}
