using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.Business.UserModule
{
    public interface IAuthUser
    {
        string Email { get; }
        string UserId { get; }
        string Roles { get; }
        string FullName { get; }
        int? OptometristFirmId { get; }
        string OptometristFirmName { get; }
        string BaseUrl { get; }
    }
}
