namespace WorkoutBuilder.Services
{
    public interface IUserContext
    {
        long? GetUserId();
        string? GetEmailAddress();
        bool CanManageWorkout(long? userId);
        bool CanManageWorkoutFavorite(long? userId);
        bool CanReadAllExercises();
        bool CanManageAllExercises();
    }
}
