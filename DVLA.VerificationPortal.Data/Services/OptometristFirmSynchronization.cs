using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using DVLA.VerificationPortal.Shared.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Services
{
    public class OptometristFirmSynchronization : IOptometristFirmSynchronization
    {
        private readonly IGenericRepository<OptometristFirm> _optometristFirmRepository;
        private readonly ILogger<OptometristFirmSynchronization> _logger;

        public OptometristFirmSynchronization(IGenericRepository<OptometristFirm> optometristFirmRepository, ILogger<OptometristFirmSynchronization> logger)
        {
            _optometristFirmRepository = optometristFirmRepository;
            _logger = logger;
        }

        public async Task<MessageResponse> SyncOptometristFirm(OptometristFirm optometristFirm)
        {
            MessageResponse response = new MessageResponse();
            try
            {
                optometristFirm = await _optometristFirmRepository.GetSingleAsync(x => x.OptometristFirmId == optometristFirm.OptometristFirmId);
                if (optometristFirm == null)
                {
                    response.Message = "Optometrist firm already synchronized";
                    return response;
                }
                optometristFirm = await _optometristFirmRepository.AddAsync(optometristFirm);
                response.Success = optometristFirm.Id > 0;
                response.Message = response.Success ? "Optometrist firm synchronized successfully." : "Failed to synchronize optometrist firm.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                response.Message = "An error occurred while synchronizing the optometrist firm.";
            }
            return response;
        }

        public async Task<List<int>> SyncOptometristFirms(List<OptometristFirm> optometristFirms)
        {
            List<int> response = new List<int>();
            try
            {
                IEnumerable<OptometristFirm> entities = await _optometristFirmRepository.GetAllAsync();
                foreach (OptometristFirm item in optometristFirms)
                {
                    OptometristFirm? firm = entities.FirstOrDefault(x => x.OptometristFirmId == item.OptometristFirmId);
                    if (firm == null)
                    {
                        var opt = await _optometristFirmRepository.AddAsync(item);
                        response.Add(opt.OptometristFirmId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return response;
        }
    }
}
