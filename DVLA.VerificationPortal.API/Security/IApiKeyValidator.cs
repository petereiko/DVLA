using DVLA.VerificationPortal.Infrastructure.Database.Entities;

namespace DVLA.VerificationPortal.API.Security
{
    public interface IApiKeyValidator
    {
        ApiClient IsValid(string apiKey);
    }
}
