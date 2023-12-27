using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Models
{
    public class HomeWorkoutModel
    {
        public string? PublicId { get; set; }
        public WorkoutGenerationResponseModel Workout { get; set; }
        public List<string> Permissions { get; set; }
    }
}
