namespace WorkoutBuilder.Services
{
    public interface IPasswordHashingService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
