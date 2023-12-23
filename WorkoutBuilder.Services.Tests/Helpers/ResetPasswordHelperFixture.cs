using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;
using WorkoutBuilder.Data;
using WorkoutBuilder.Services.Impl.Helpers;
using WorkoutBuilder.Services.Tests.TestUtilities;

namespace WorkoutBuilder.Services.Tests.Helpers
{
    public class ResetPasswordHelperFixture
    {
        [TestFixture]
        public class When_Resetting
        {
            [Test]
            public async Task Should_Reset()
            {
                // Arrange
                var users = new[] { new User { EmailAddress = "user@example.com", Id = 100 } };
                var userRepository = new TestRepo<User>(users);

                var userResetService = A.Fake<IUserResetPasswordService>(opt => opt.Strict());
                A.CallTo(() => userResetService.Create(100L, "1.1.1.1")).Returns("new-id");

                var contextAccessor = A.Fake<IActionContextAccessor>();
                var context = A.Fake<HttpContext>();
                A.CallTo(() => context.Connection.RemoteIpAddress).Returns(IPAddress.Parse("1.1.1.1"));
                A.CallTo(() => contextAccessor.ActionContext).Returns(new ActionContext { HttpContext = context });

                var emailService = A.Fake<IEmailService>();
                var urlService = A.Fake<IUrlBuilder>();
                A.CallTo(() => urlService.Action(null!, null!, null!)).WithAnyArguments().Returns("https://google.com");

                var resetPasswordHelper = new ResetPasswordHelper
                {
                    ActionContextAccessor = contextAccessor,
                    UserRepository = userRepository,
                    UserResetPasswordService = userResetService,
                    EmailService = emailService,
                    UrlBuilder = urlService
                };

                // Act
                var result = await resetPasswordHelper.Reset("user@example.com");

                // Assert
                Assert.IsTrue(result);
                A.CallTo(() => userResetService.Create(100L, "1.1.1.1")).MustHaveHappened();
                A.CallTo(() => emailService.Send("user@example.com", A<string>._, A<string>._)).MustHaveHappened();
            }

            [Test]
            public async Task Should_Reject_Unknown_Email()
            {
                // Arrange
                var users = new[] { new User { EmailAddress = "user@example.com", Id = 100 } };
                var userRepository = new TestRepo<User>(users);

                var userResetService = A.Fake<IUserResetPasswordService>();
                var emailService = A.Fake<IEmailService>();

                var resetPasswordHelper = new ResetPasswordHelper
                {
                    UserRepository = userRepository,
                    UserResetPasswordService = userResetService,
                    EmailService = emailService
                };

                // Act
                var result = await resetPasswordHelper.Reset("bogus@email.com");

                // Assert
                Assert.IsFalse(result);
                A.CallTo(() => userResetService.Create(0L, null!)).WithAnyArguments().MustNotHaveHappened();
                A.CallTo(() => emailService.Send(null!, null!, null!)).WithAnyArguments().MustNotHaveHappened();
            }
        }
    }
}