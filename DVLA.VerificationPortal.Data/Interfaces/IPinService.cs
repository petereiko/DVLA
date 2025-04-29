using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVLA.VerificationPortal.Shared.Responses;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IPinService
    {
        Task<MessageResponse> VendPinsAsync();
    }
}
