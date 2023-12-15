using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutBuilder.Data;

namespace WorkoutBuilder.Services.Impl
{
    public class UserResetPasswordService
    {
        public IRepository<User> UserRepository { get; init; }
        public IRepository<UserPasswordResetRequest> UserPasswordResetRequestRepository { get; init; }
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

        public void Complete(string publicId, string newPassword) { }
    }
}
