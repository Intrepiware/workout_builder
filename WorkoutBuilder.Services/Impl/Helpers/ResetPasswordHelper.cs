using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutBuilder.Data;

namespace WorkoutBuilder.Services.Impl.Helpers
{
    public class ResetPasswordHelper
    {
        public IRepository<User> UserRepository { protected get; init; }
        public IUserResetPasswordService UserResetPasswordService { protected get; init; }
        public IEmailService EmailService { protected get; init; }

        public async Task<bool> Reset(string emailAddress)
        {
            var user = UserRepository.GetAll().Where(x => x.EmailAddress == emailAddress).SingleOrDefault();
            if (user == null)
                return false;

            var resetId = await UserResetPasswordService.Create(user.Id);
            if (string.IsNullOrEmpty(resetId))
                return false;

            var message = "Hello! Someone (hopefully you) has requested a password reset to workoutbuild.com. Please"

        }
    }
}
