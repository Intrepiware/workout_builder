using System.ComponentModel.DataAnnotations;

namespace WorkoutBuilder.Models
{
    public class UserResetPasswordModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string Confirm { get; set; }
        public string PublicId { get; set; }
    }
}
