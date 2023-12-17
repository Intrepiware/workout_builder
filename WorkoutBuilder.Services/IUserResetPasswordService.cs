namespace WorkoutBuilder.Services
{

    public interface IUserResetPasswordService
    {
        Task<string> Create(long userId, string ipAddress);
        bool Retrieve(string publicId);
        Task Complete(string publicId, string newPassword);
    }
}
