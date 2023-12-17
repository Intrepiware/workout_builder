using BotDetect.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.Controllers
{
    public class UsersController : Controller
    {
        public IUserResetPasswordService UserResetPasswordService { protected get; init; }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidationActionFilter("CaptchaCode", "ContactFormCaptcha", "Incorrect Captcha, please try again.")]
        public IActionResult ForgotPassword(UserForgotPasswordModel data)
        {
            if (!ModelState.IsValid)
                return View(data);

            UserResetPasswordService.Create()
        }

    }
}
