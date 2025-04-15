using Microsoft.AspNetCore.Mvc;

namespace DVLA.VerificationPortal.API.Security
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute()
            : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
