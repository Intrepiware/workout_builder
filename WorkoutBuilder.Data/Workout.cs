using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("Workout")]
    public class Workout
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(255)]
        public string PublicId { get; set; }
        public long? UserId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        public string Body { get; set; }
        public bool IsFavorite { get; set; }
        public virtual User User { get; set; }
    }
}
