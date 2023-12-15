using Microsoft.AspNet.Identity;

namespace WorkoutBuilder.Services.Impl
{
    public class Pbkdf2PasswordHashingService
    {
        public string Hash(string password) => new PasswordHasher().HashPassword(password);

        public bool Verify(string password, string hash) => new PasswordHasher().VerifyHashedPassword(hash, password) != PasswordVerificationResult.Failed;
    }
}