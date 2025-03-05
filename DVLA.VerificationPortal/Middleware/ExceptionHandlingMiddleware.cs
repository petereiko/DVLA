namespace DVLA.VerificationPortal.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Continue processing the request
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                // Store the error message in HttpContext.Items
                context.Items["ErrorMessage"] = ex.Message;

                // Redirect to a custom error page or the same page
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
