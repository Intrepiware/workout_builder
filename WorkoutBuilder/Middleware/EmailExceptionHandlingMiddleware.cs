using WorkoutBuilder.Services;

namespace WorkoutBuilder.Middleware
{
    public class EmailExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public EmailExceptionHandlingMiddleware(RequestDelegate next, IEmailService emailService, IConfiguration configuration)
        {
            _next = next;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(Exception ex)
            {
                try
                {
                    _emailService.Send(_configuration["ErrorEmailAddress"], $"[{nameof(WorkoutBuilder)}] - {ex.Message}", ex.ToString());
                }
                catch { }

                throw;
            }
        }
    }
}
