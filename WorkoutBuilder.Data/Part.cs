using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("Part")]
    public class Part
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMuscle { get; set; }
    }
}
