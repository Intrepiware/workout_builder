using WorkoutBuilder.Services.Models;

namespace WorkoutBuilder.Services
{
    public interface IWorkoutGenerator
    {
        WorkoutGenerationResponseModel Generate(WorkoutGenerationRequestModel request);
    }
}
