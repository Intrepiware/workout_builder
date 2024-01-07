using WorkoutBuilder.Data;

namespace WorkoutBuilder.Services
{
    public interface IExerciseService
    {
        List<Exercise> Search(int take, int skip, string? name = null, string? focus = null, string? equipment = null);
        Task<long> Add(Exercise exercise);
        Task Update(Exercise exercise);
    }
}
