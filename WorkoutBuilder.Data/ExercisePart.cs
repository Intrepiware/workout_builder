using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutBuilder.Data
{
    [Table("ExercisePart")]
    [PrimaryKey(nameof(ExerciseId), nameof(PartId))]
    public class ExercisePart
    {
        public long ExerciseId { get; set; }
        public long PartId { get; set; }
        public virtual Exercise Exercise { get; set; }
        public virtual Part Part { get; set; }
    }
}
