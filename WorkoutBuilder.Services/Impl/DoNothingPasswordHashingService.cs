namespace WorkoutBuilder.Services.Impl
{
    public class DoNothingPasswordHashingService : IPasswordHashingService
    {
        public string Hash(string password) => password;
        public bool Verify(string password, string hash) => password == hash;
    }
}
