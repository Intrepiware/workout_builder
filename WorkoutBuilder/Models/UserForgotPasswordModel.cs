using System.ComponentModel.DataAnnotations;

namespace WorkoutBuilder.Models
{
    public class UserForgotPasswordModel
    {
        [EmailAddress, Required]
        public string EmailAddress { get; set; }
        [Required]
        public string CaptchaCode { get; set; }

    }
}
