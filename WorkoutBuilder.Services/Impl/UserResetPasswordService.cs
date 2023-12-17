﻿using WorkoutBuilder.Data;

namespace WorkoutBuilder.Services.Impl
{
    public class UserResetPasswordService : IUserResetPasswordService
    {
        public IRepository<User> UserRepository { init; protected get; }
        public IRepository<UserPasswordResetRequest> UserPasswordResetRequestRepository { init; protected get; }
        public IPasswordHashingService PasswordHashingService { init; protected get; }
        public async Task<string> Create(long userId)
        {
            if (UserRepository.GetById(userId) == null)
                return null;

            var request = new UserPasswordResetRequest
            {
                CreateDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddHours(2),
                PublicId = Guid.NewGuid().ToString("n"),
                UserId = userId
            };

            await UserPasswordResetRequestRepository.Add(request);
            return request.PublicId;
        }

        public bool Retrieve(string publicId)
        {
            var request = UserPasswordResetRequestRepository.GetAll().Where(x => x.PublicId == publicId).SingleOrDefault();
            if (request == null || DateTime.UtcNow < request.CreateDate || DateTime.UtcNow > request.ExpireDate || request.CompleteDate.HasValue)
                return false;

            return true;
        }

        public async Task Complete(string publicId, string newPassword)
        {
            var request = UserPasswordResetRequestRepository.GetAll().Where(x => x.PublicId == publicId).SingleOrDefault();
            if (request == null || DateTime.UtcNow < request.CreateDate || DateTime.UtcNow > request.ExpireDate || request.CompleteDate.HasValue)
                throw new ArgumentException("Password reset request does not exist or has expired.");

            var user = await UserRepository.GetById(request.UserId);
            user.Password = PasswordHashingService.Hash(newPassword);
            user.PasswordResetDate = DateTime.UtcNow;
            user.LockDate = null;
            await UserRepository.Update(user);
        }
    }
}
