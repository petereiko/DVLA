using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DVLA.VerificationPortal.API.Security
{
    public class ApiKeyAuthorizationFilter: IAuthorizationFilter
    {
        private const string ApiKeyHeaderName = "X-API-Key";

        private readonly IApiKeyValidator _apiKeyValidator;

        public ApiKeyAuthorizationFilter(IApiKeyValidator apiKeyValidator)
        {
            _apiKeyValidator = apiKeyValidator;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

            if (!string.IsNullOrEmpty(apiKey))
            {
                ApiClient apiClient = _apiKeyValidator.IsValid(apiKey);
                if (apiClient==null)
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    context.HttpContext.Items.Add("ApiKey", apiKey);
                    context.HttpContext.Items.Add("Name", apiClient.Name);
                }
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
