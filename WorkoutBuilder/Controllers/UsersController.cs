using BotDetect.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services.Impl.Helpers;

namespace WorkoutBuilder.Controllers
{
    public class UsersController : Controller
    {
        public IResetPasswordHelper ResetPasswordHelper { protected get; init; }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "ForgotPasswordCaptcha", "Incorrect Captcha, please try again.")]
        public async Task<IActionResult> ForgotPassword(UserForgotPasswordModel data)
        {
            if (!ModelState.IsValid)
                return View(data);

            await ResetPasswordHelper.Reset(data.EmailAddress);

            ViewBag.Success = "If the provided email address belongs to an existing account, then a message has been sent to that address with further instructions.";
            ModelState.Clear();
            MvcCaptcha.ResetCaptcha("ForgotPasswordCaptcha");
            return View(new UserForgotPasswordModel());

        }

    }
}
