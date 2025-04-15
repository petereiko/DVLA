using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLA.VerificationPortal.Application.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly IGenericRepository<ApiClient> _apiClientRepository;
        private readonly ILogger<ApiClientService> _logger;
        public ApiClientService(IGenericRepository<ApiClient> apiClientRepository, ILogger<ApiClientService> logger)
        {
            _apiClientRepository = apiClientRepository;
            _logger = logger;
        }


        public async Task<ApiClient> AuthenticateAsync(string secret)
        {
            ApiClient? client = null;
            try
            {
                client = await _apiClientRepository.GetSingleAsync(x => x.ApiKey == secret);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message, ex);
                
            }
            return client;
        }
    }
}
