namespace WorkoutBuilder.Services
{
    public interface IUserContext
    {
        long? GetUserId();
        string? GetEmailAddress();
    }
}
