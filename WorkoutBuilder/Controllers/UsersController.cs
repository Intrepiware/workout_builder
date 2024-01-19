using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WorkoutBuilder.Data;
using WorkoutBuilder.Middleware;
using WorkoutBuilder.Models;
using WorkoutBuilder.Services;
using WorkoutBuilder.Services.Impl.Helpers;
using IAuthenticationService = WorkoutBuilder.Services.IAuthenticationService;

namespace WorkoutBuilder.Controllers
{

    public class UsersController : Controller
    {
        public IResetPasswordHelper ResetPasswordHelper { protected get; init; } = null!;
        public IUserResetPasswordService ResetPasswordService { protected get; init; } = null!;
        public IRepository<UserPasswordResetRequest> PasswordResetRepository { protected get; init; } = null!;
        public IAuthenticationService AuthenticationService { protected get; init; } = null!;

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ServiceFilter(typeof(ValidateRecaptchaServiceFilter))]
        public async Task<IActionResult> ForgotPassword(UserForgotPasswordModel data)
        {
            if (!ModelState.IsValid)
                return View(data);

            await ResetPasswordHelper.Reset(data.EmailAddress);

            ViewBag.Success = "If the email address belongs to an account, a message has been sent with further instructions.";
            ModelState.Clear();
            return View(new UserForgotPasswordModel());

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            if (await AuthenticationService.Login(Username, Password))
                return RedirectToAction("Index", "Home");

            ViewBag.Error = "Incorrect Username/Password.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
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
