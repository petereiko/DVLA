using DVLA.VerificationPortal.Domain.Entities;

namespace DVLA.VerificationPortal.API.Security
{
    public interface IApiKeyValidator
    {
        ApiClient IsValid(string apiKey);
    }
}
