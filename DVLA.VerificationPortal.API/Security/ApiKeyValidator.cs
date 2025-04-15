
using DVLA.VerificationPortal.API.Security;
using DVLA.VerificationPortal.Application.Interfaces;
using DVLA.VerificationPortal.Domain.Entities;

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
