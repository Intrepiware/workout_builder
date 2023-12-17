using System.ComponentModel.DataAnnotations;

namespace WorkoutBuilder.Models
{
    public class UserForgotPasswordModel
    {
        [EmailAddress]
        public string EmailAddress { get; set; }
        public string CaptchaCode { get; set; }

    }
}
