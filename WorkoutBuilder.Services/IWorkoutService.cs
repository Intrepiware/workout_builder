using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services
{

    public interface IWorkoutService
    {
        Task<string?> Create(WorkoutGenerationResponseModel generatedWorkout);
        Task<Workout> ToggleFavorite(string publicId);
    }
}
