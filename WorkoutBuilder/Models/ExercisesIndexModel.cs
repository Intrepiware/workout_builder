using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Models
{
    public class ExercisesIndexModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public string Equipment { get; set; }
        public long FocusId { get; set; }
        public List<string> EquipmentOptions { get; set; } = null!;
        public List<Focus> FocusOptions { get; set; } = null!;
    }
}
