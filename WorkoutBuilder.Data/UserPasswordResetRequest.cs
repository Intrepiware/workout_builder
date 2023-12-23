using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("UserPasswordResetRequest")]
    public class UserPasswordResetRequest
    {
        [Key]
        public long Id { get; set; }

        public long UserId { get; set; }

        [Required, MaxLength(255)]
        public string PublicId { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public DateTime ExpireDate { get; set; }
        [Required, MaxLength(50)]
        public string IpAddress { get; set; }
        public DateTime? CompleteDate { get; set; }

        public virtual User User { get; set; }
    }
}
