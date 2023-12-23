using Microsoft.AspNetCore.Mvc.Infrastructure;
using WorkoutBuilder.Data;

namespace WorkoutBuilder.Services.Impl.Helpers
{
    public interface IResetPasswordHelper
    {
        Task<bool> Reset(string emailAddress);
    }

    public class ResetPasswordHelper : IResetPasswordHelper
    {
        public IRepository<User> UserRepository { protected get; init; } = null!;
        public IUserResetPasswordService UserResetPasswordService { protected get; init; } = null!;
        public IEmailService EmailService { protected get; init; } = null!;
        public IUrlBuilder UrlBuilder { protected get; init; } = null!;
        public IActionContextAccessor ActionContextAccessor { init; protected get; } = null!;

        public async Task<bool> Reset(string emailAddress)
        {
            var user = UserRepository.GetAll().Where(x => x.EmailAddress == emailAddress).SingleOrDefault();
            if (user == null)
                return false;

            var resetId = await UserResetPasswordService.Create(user.Id, ActionContextAccessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString());
            if (string.IsNullOrEmpty(resetId))
                return false;

            var resetLink = UrlBuilder.Action("ResetPassword", "Users", new { id = resetId });
            var message = $"<p>Hello! Someone (hopefully you) has requested a password reset to workoutbuild.com. Please use the following link to reset your password:<br /><br /><a href=\"{resetLink}\">{resetLink}</a><br /><br />- The Workout Builder Team</a>";
            EmailService.Send(emailAddress, "Workout Builder - Reset Password Request", message);

            return true;
        }
    }
}
