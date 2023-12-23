using Microsoft.Extensions.Configuration;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.Services.Tests
{
    [TestFixture]
    public class SendgridEmailServiceFixture
    {
        [Test]
        [Ignore("Integration Test, sends actual email")]
        public void Should_Send_Email()
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["SendgridApiKey"]).Returns("YOUR_API_KEY");
            A.CallTo(() => config["SendgridFromAddress"]).Returns("YOUR_AUTHORIZED_FROM_ADDRESS");
            A.CallTo(() => config["SendgridFromName"]).Returns("Sender Name");
            var sendgrid = new SendgridEmailService { Configuration = config };
            sendgrid.Send("yourrecipient@gmail.com", "Test Email From Unit Tests", "<b>this</b><br /><i>is</i><br />a<br />test");
            Assert.Pass();
        }

        [Test]
        public void Should_Throw()
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config["SendgridApiKey"]).Returns("fake_key");
            A.CallTo(() => config["SendgridFromAddress"]).Returns("f@ke.com");
            A.CallTo(() => config["SendgridFromName"]).Returns("No Reply");
            var sendgrid = new SendgridEmailService { Configuration = config };

            var ex = Assert.Throws<ApplicationException>(() => sendgrid.Send("em@il.com", "This Email Should Not Send", "Test message"));

            StringAssert.Contains("error", ex.Message);
        }
    }
}
