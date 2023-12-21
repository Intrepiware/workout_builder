using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl;

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
                var userRepository = A.Fake<IRepository<User>>();
                var passwordResetRepository = A.Fake<IRepository<UserPasswordResetRequest>>();
                UserPasswordResetRequest capturedRequest = null!;

                A.CallTo(() => userRepository.GetById(1L)).Returns(new User { });
                A.CallTo(() => passwordResetRepository.Add(A<UserPasswordResetRequest>._))
                    .Invokes((UserPasswordResetRequest req) => capturedRequest = req);

                var resetPasswordService = new UserResetPasswordService
                {
                    UserRepository = userRepository,
                    UserPasswordResetRequestRepository = passwordResetRepository
                };

                // Act
                var result = await resetPasswordService.Create(1, "1.2.3.4");


                // Assert
                A.CallTo(() => userRepository.GetById(1L)).MustHaveHappened();
                Assert.IsNotNull(capturedRequest);
                Assert.IsNotNull(result);
                Assert.That(capturedRequest.PublicId, Is.EqualTo(result));
                Assert.That(DateTime.UtcNow.AddMinutes(121), Is.GreaterThan(capturedRequest.ExpireDate));
                Assert.That("1.2.3.4", Is.EqualTo(capturedRequest.IpAddress));
            }

            [Test]
            public async Task Should_Reject_Invalid_UserId()
            {

                // Arrange
                var userRepository = A.Fake<IRepository<User>>(opt => opt.Strict());
                var passwordResetRepository = A.Fake<IRepository<UserPasswordResetRequest>>();
                UserPasswordResetRequest capturedRequest = null!;

                A.CallTo(() => userRepository.GetById(2L)).Returns((User)null!);
                A.CallTo(() => passwordResetRepository.Add(A<UserPasswordResetRequest>._))
                    .Invokes((UserPasswordResetRequest req) => capturedRequest = req);

                var resetPasswordService = new UserResetPasswordService
                {
                    UserRepository = userRepository,
                    UserPasswordResetRequestRepository = passwordResetRepository
                };

                // Act
                var result = await resetPasswordService.Create(2, "1.2.3.4");

                // Assert
                A.CallTo(() => userRepository.GetById(2L)).MustHaveHappened();
                Assert.IsNull(result);
                A.CallTo(() => passwordResetRepository.Add(A<UserPasswordResetRequest>._)).MustNotHaveHappened();
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
                var passwordRequestRepository = A.Fake<IRepository<UserPasswordResetRequest>>();
                A.CallTo(() => passwordRequestRepository.GetAll()).Returns(new[] { request }.AsQueryable());

                var service = new UserResetPasswordService { UserPasswordResetRequestRepository = passwordRequestRepository };

                var result = service.Retrieve("secret");

                Assert.IsTrue(result);

            }

            [Test]
            public void Should_Not_Retrieve_Expired()
            {
                var now = DateTime.UtcNow;
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(-3), UserId = 1 };
                var passwordRequestRepository = A.Fake<IRepository<UserPasswordResetRequest>>();
                A.CallTo(() => passwordRequestRepository.GetAll()).Returns(new[] { request }.AsQueryable());

                var service = new UserResetPasswordService { UserPasswordResetRequestRepository = passwordRequestRepository };

                var result = service.Retrieve("secret");

                Assert.IsFalse(result);

            }

            [Test]
            public void Should_Not_Retrieve_Unknown()
            {
                var now = DateTime.UtcNow;
                var request = new UserPasswordResetRequest { PublicId = "secret", CreateDate = now.AddMinutes(-5), ExpireDate = now.AddMinutes(5), UserId = 1 };
                var passwordRequestRepository = A.Fake<IRepository<UserPasswordResetRequest>>();
                A.CallTo(() => passwordRequestRepository.GetAll()).Returns(new[] { request }.AsQueryable());

                var service = new UserResetPasswordService { UserPasswordResetRequestRepository = passwordRequestRepository };

                var result = service.Retrieve("badkey");

                Assert.IsFalse(result);

            }
        }
    }
}
