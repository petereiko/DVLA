using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Shared.DTOs;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IAuthUser
    {
        string Email { get; }
        string UserId { get; }
        string Role { get; }
    }
}
