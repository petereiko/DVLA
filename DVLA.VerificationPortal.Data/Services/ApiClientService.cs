using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;
using DVLA.VerificationPortal.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
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
        private readonly IGenericRepository<ApiAuditLog> _apiAuditLogRepository;
        private readonly ILogger<ApiClientService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiClientService(IGenericRepository<ApiClient> apiClientRepository, ILogger<ApiClientService> logger, IHttpContextAccessor httpContextAccessor, IGenericRepository<ApiAuditLog> apiAuditLogRepository)
        {
            _apiClientRepository = apiClientRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _apiAuditLogRepository = apiAuditLogRepository;
        }

        public string? GetApiKey()
        {
            var headers = _httpContextAccessor.HttpContext?.Request?.Headers;

            if (headers != null && headers.TryGetValue("ApiKey", out var apiKey))
            {
                return apiKey.ToString();
            }

            return null;
        }

        //public string? ApiName => _httpContextAccessor.HttpContext?.Request.Cookies["ApiName"];


        public string? ApiKey
        {
            get
            {
                var headers = _httpContextAccessor.HttpContext?.Request?.Headers;

                if (headers != null && headers.TryGetValue("X-API-KEY", out var apiKey))
                {
                    return apiKey.ToString();
                }

                return null;
            }
        }

        public async Task<ApiClient?> AuthenticateAsync(string secret)
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

        public async Task AuditLogAsync(string controller, string action, string? apiKey)
        {
            IEnumerable<ApiClient> clients = await _apiClientRepository.FilterAsync(x => x.ApiKey == apiKey);
            ApiClient? client = clients.FirstOrDefault();

            ApiAuditLog log = new() { Action = action, Controller = controller, ApiClientId = client.Id, CreatedDate = DateTime.UtcNow };
            await _apiAuditLogRepository.AddAsync(log);
        }
    }
}
