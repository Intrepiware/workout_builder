using System.ComponentModel.DataAnnotations;

namespace WorkoutBuilder.Models
{
    public class HomeContactRequestModel
    {
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required, MaxLength(255)]
        public string Location { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required, MaxLength(255)]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
