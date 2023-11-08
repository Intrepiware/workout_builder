using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("Exercise")]
    public class Exercise
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        [Required, MaxLength(255)]
        public string Equipment { get; set; }
        public long FocusId { get; set; }
        public string Notes { get; set; }

        public virtual Focus Focus { get; set; }
    }
}
