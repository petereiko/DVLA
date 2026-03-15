
using DVLA.VerificationPortal.API.Security;
using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Repositories;

namespace AtlasWallet.Api.Security
{
    public class ApiKeyValidator : IApiKeyValidator
    {
        private readonly IApiClientService _apiClientService;

        public ApiKeyValidator(IApiClientService apiClientService)
        {
            _apiClientService = apiClientService;
        }
        public ApiClient IsValid(string apiKey)
        {
            ApiClient result = _apiClientService.AuthenticateAsync(apiKey).GetAwaiter().GetResult();
            return result;
        }

        
    }
}
