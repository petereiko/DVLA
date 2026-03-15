using DVLA.VerificationPortal.Infrastructure.Database.Entities;
using DVLA.VerificationPortal.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DVLA.VerificationPortal.CustomAttributes
{
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "API Key was not provided."
                };
                return;
            }

            var validator = context.HttpContext.RequestServices.GetRequiredService<IApiClientService>();
            ApiClient? client = await validator.AuthenticateAsync(extractedApiKey);
            if (client==null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Invalid API Key."
                };
                return;
            }

            var response = context.HttpContext.Response;

            response.Cookies.Append("ApiName", client.Name ?? "", new CookieOptions { HttpOnly = true, Secure = true });
            response.Cookies.Append("ApiKey", client.ApiKey ?? "", new CookieOptions { HttpOnly = true, Secure = true });
            response.Cookies.Append("ApiId", client.Id.ToString() ?? "", new CookieOptions { HttpOnly = true, Secure = true });


            await next();
        }
    }

}
