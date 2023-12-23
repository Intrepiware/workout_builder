using BotDetect.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuilder.Data;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.Impl.Helpers;

namespace WorkoutBuilder.Controllers
{

    public class UsersController : Controller
    {
        public IResetPasswordHelper ResetPasswordHelper { protected get; init; } = null!;
        public IUserResetPasswordService ResetPasswordService { protected get; init; } = null!;
        public IRepository<UserPasswordResetRequest> PasswordResetRepository { protected get; init; } = null!;

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CaptchaValidationActionFilter("CaptchaCode", "ForgotPasswordCaptcha", "Incorrect Captcha, please try again.")]
        public async Task<IActionResult> ForgotPassword(UserForgotPasswordModel data)
        {
            if (!ModelState.IsValid)
                return View(data);

            await ResetPasswordHelper.Reset(data.EmailAddress);

            ViewBag.Success = "If the email address belongs to an account, a message has been sent with further instructions.";
            ModelState.Clear();
            MvcCaptcha.ResetCaptcha("ForgotPasswordCaptcha");
            return View(new UserForgotPasswordModel());

        }

        [HttpGet]
        public IActionResult ResetPassword(string id)
        {
            if (ResetPasswordService.Retrieve(id))
                return View(new UserResetPasswordModel { PublicId = id });

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(UserResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await ResetPasswordService.Complete(model.PublicId, model.Password);
            ViewBag.Success = "If the email address belongs to an account, a message has been sent with further instructions.";

            ModelState.Clear();
            return View("ResetSuccess");
        }
    }
}
