using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("Timing")]
    public class Timing
    {
        [Key]
        public long Id { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; }
        public byte Stations { get; set; }
        [MaxLength(255)]
        public string StationTiming { get; set; }
        public string? Notes { get; set; }
        public string? CustomGenerator { get; set; }
    }
}
