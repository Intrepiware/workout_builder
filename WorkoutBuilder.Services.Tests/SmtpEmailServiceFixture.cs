using Microsoft.Extensions.Configuration;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.Services.Tests
{
    [TestFixture]
    public class SmtpEmailServiceFixture
    {
        [Test]
        [Ignore("Integration Test, sends actual email")]
        public void Should_Send_Email()
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["SmtpServer"]).Returns("smtp.gmail.com");
            A.CallTo(() => config["SmtpUsername"]).Returns("yourlogin@gmail.com");
            A.CallTo(() => config["SmtpPassword"]).Returns("yourpassword");
            A.CallTo(() => config["SmtpFromAddress"]).Returns("yourlogin@gmail.com");
            var smtp = new SmtpEmailService() { Configuration = config };
            smtp.Send("fake@example.com", "Test Email From Unit Tests", "this\nis\na\ntest\nemail");
            Assert.Pass();
        }
    }
}
