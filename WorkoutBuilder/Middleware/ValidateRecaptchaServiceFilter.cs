using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WorkoutBuilder.Middleware
{
    public class ValidateRecaptchaServiceFilter : IAsyncActionFilter
    {
        private static HttpClient RecaptchaHttpClient = null!;
        public IConfiguration Configuration { protected get; init; } = null!;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var solved = false;
            var gCaptchaToken = context.HttpContext.Request.Form["g-recaptcha-response"];
            if (!string.IsNullOrEmpty(gCaptchaToken))
            {
                RecaptchaHttpClient ??= new HttpClient();
                var secretKey = Configuration["GoogleRecaptchaSecretKey"];
                var formParams = new Dictionary<string, string> { ["secret"] = secretKey, ["response"] = gCaptchaToken };
                var encodedContent = new FormUrlEncodedContent(formParams);
                var response = await RecaptchaHttpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", encodedContent);

                try
                {
                    solved = response.IsSuccessStatusCode && (await response.Content.ReadFromJsonAsync<CaptchaResponse>()).Success;
                }
                catch { }
            }

            if (!solved)
            {
                context.ModelState.AddModelError("CaptchaCode", "Invalid Captcha Code. Please try again.");
                context.Result = new ViewResult() { ViewName = "Contact" };
            }
            else
                await next();
        }

        private class CaptchaResponse
        {
            public bool Success { get; set; }
            public string Challenge_Ts { get; set; }
            public string Hostname { get; set; }
            public string[] ErrorCodes { get; set; }
        }

    }
}
