using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;
using WorkoutBuilder.Services.Tests.TestUtilities;

namespace WorkoutBuilder.Services.Tests
{
    public class UserResetPasswordServiceFixture
    {
        [TestFixture]
        public class When_Creating
        {
            [Test]
            public async Task Should_Create()
            {
                // Arrange
                var userRepository = new TestRepo<User>(new[] { new User { Id = 1 } });
                var passwordResetRepository = new TestRepo<UserPasswordResetRequest>();

                var resetPasswordService = new UserResetPasswordService
                {
                    UserRepository = userRepository,
                    UserPasswordResetRequestRepository = passwordResetRepository
                };

                // Act
                var result = await resetPasswordService.Create(1, "1.2.3.4");


                // Assert
                Assert.That(passwordResetRepository.AddedItems.Count, Is.EqualTo(1));
                Assert.IsNotNull(result);
                var newRequest = passwordResetRepository.AddedItems.Single();
                Assert.That(newRequest.PublicId, Is.EqualTo(result));
                Assert.That(DateTime.UtcNow.AddMinutes(121), Is.GreaterThan(newRequest.ExpireDate));
                Assert.That(newRequest.IpAddress, Is.EqualTo("1.2.3.4"));
            }

            [Test]
            public async Task Should_Reject_Invalid_UserId()
            {

                // Arrange
                var userRepository = new TestRepo<User>();
                var passwordResetRepository = new TestRepo<UserPasswordResetRequest>();

                var resetPasswordService = new UserResetPasswordService
                {
                    UserRepository = userRepository,
                    UserPasswordResetRequestRepository = passwordResetRepository
                };

                // Act
                var result = await resetPasswordService.Create(2, "1.2.3.4");

                // Assert
                Assert.IsNull(result);
                Assert.That(passwordResetRepository.AddedItems.Count, Is.EqualTo(0));
            }
        }

        [TestFixture]
        public class When_Retrieving
        {

            [Test]
            public void Should_Retrieve_Valid_Request()
            {
                var now = DateTime.UtcNow;
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(5), UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });

                var service = new UserResetPasswordService { UserPasswordResetRequestRepository = passwordRequestRepository };

                var result = service.Retrieve("secret");

                Assert.IsTrue(result);

            }

            [Test]
            public void Should_Not_Retrieve_Expired()
            {
                var now = DateTime.UtcNow;
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(-3), UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });

                var service = new UserResetPasswordService { UserPasswordResetRequestRepository = passwordRequestRepository };

                var result = service.Retrieve("secret");

                Assert.IsFalse(result);

            }

            [Test]
            public void Should_Not_Retrieve_Unknown()
            {
                var now = DateTime.UtcNow;
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(5), UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });

                var service = new UserResetPasswordService { UserPasswordResetRequestRepository = passwordRequestRepository };

                var result = service.Retrieve("badkey");

                Assert.IsFalse(result);

            }
        }

        [TestFixture]
        public class When_Completing
        {
            [Test]
            public async Task Should_Complete()
            {
                var now = DateTime.UtcNow;

                // Arrange
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(5), UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });
                var userRepository = new TestRepo<User>(new[] { new User { Id = 1, LockDate = now } });

                var hashingService = A.Fake<IPasswordHashingService>();
                A.CallTo(() => hashingService.Hash(null!)).WithAnyArguments()
                    .Returns("hashed_password");

                // Act
                var service = new UserResetPasswordService { UserRepository = userRepository, UserPasswordResetRequestRepository = passwordRequestRepository, PasswordHashingService = hashingService };
                await service.Complete("secret", "newPassword");

                // Assert
                var capturedUser = userRepository.UpdatedItems.SingleOrDefault();
                var capturedRequest = passwordRequestRepository.UpdatedItems.SingleOrDefault();
                Assert.IsNotNull(capturedUser);
                Assert.IsNotNull(capturedRequest);
                Assert.That(capturedUser.Password, Is.EqualTo("hashed_password"));
                Assert.IsNull(capturedUser.LockDate);
                Assert.That(now.AddMinutes(-1), Is.LessThan(capturedUser.PasswordResetDate));
                Assert.That(now.AddMinutes(-1), Is.LessThan(capturedRequest.CompleteDate));
            }

            [Test]
            public void Should_Reject_Expired()
            {
                var now = DateTime.UtcNow;

                // Arrange
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(-3), UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });
                
                var userRepository = A.Fake<IRepository<User>>(opt => opt.Strict());

                var hashingService = A.Fake<IPasswordHashingService>();
                var service = new UserResetPasswordService { UserRepository = userRepository, UserPasswordResetRequestRepository = passwordRequestRepository, PasswordHashingService = hashingService };

                // Act / Assert

                var exception = Assert.ThrowsAsync<ArgumentException>(async () => await service.Complete("secret", "newPassword"));
                StringAssert.Contains("expired", exception.Message);
                Assert.That(passwordRequestRepository.UpdatedItems.Count, Is.EqualTo(0));
                A.CallTo(() => userRepository.Update(null!)).WithAnyArguments().MustNotHaveHappened();
            }


            [Test]
            public void Should_Reject_Completed()
            {
                var now = DateTime.UtcNow;

                // Arrange
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(5), CompleteDate = now, UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });

                var userRepository = A.Fake<IRepository<User>>(opt => opt.Strict());
                var hashingService = A.Fake<IPasswordHashingService>();
                var service = new UserResetPasswordService { UserRepository = userRepository, UserPasswordResetRequestRepository = passwordRequestRepository, PasswordHashingService = hashingService };

                // Act / Assert

                var exception = Assert.ThrowsAsync<ArgumentException>(async () => await service.Complete("secret", "newPassword"));
                Assert.That(passwordRequestRepository.UpdatedItems.Count, Is.EqualTo(0));
                A.CallTo(() => userRepository.Update(null!)).WithAnyArguments().MustNotHaveHappened();
            }

            [Test]
            public void Should_Reject_NotFound()
            {
                var now = DateTime.UtcNow;

                // Arrange
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(5), UserId = 1 };
                var passwordRequestRepository = new TestRepo<UserPasswordResetRequest>(new[] { request });

                var userRepository = A.Fake<IRepository<User>>(opt => opt.Strict());
                var hashingService = A.Fake<IPasswordHashingService>();
                var service = new UserResetPasswordService { UserRepository = userRepository, UserPasswordResetRequestRepository = passwordRequestRepository, PasswordHashingService = hashingService };

                // Act / Assert

                var exception = Assert.ThrowsAsync<ArgumentException>(async () => await service.Complete("bogus", "newPassword"));
                Assert.That(passwordRequestRepository.UpdatedItems.Count, Is.EqualTo(0));
                A.CallTo(() => userRepository.Update(null!)).WithAnyArguments().MustNotHaveHappened();
            }
        }
    }
}
