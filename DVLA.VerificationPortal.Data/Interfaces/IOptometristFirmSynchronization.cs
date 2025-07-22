using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Interfaces
{
    public interface IOptometristFirmSynchronization
    {
        Task<List<int>> SyncOptometristFirms(List<OptometristFirm> optometristFirms);
        Task<MessageResponse> SyncOptometristFirm(OptometristFirm optometristFirm);
    }
}
