using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutBuilder.Services.Impl;

namespace WorkoutBuilder.Services.Tests
{
    [TestFixture]
    public class Pbkdf2PasswordHashingServiceFixture
    {
        [Test]
        public void Should_Hash_And_Verify()
        {
            var password = Guid.NewGuid().ToString("n");
            var hashingService = new Pbkdf2PasswordHashingService();

            var hash = hashingService.Hash(password);

            Assert.IsTrue(hashingService.Verify(password, hash));
        }

        [Test]
        public void Should_Reject_Wrong_Password()
        {
            var password = Guid.NewGuid().ToString("n");
            var hashingService = new Pbkdf2PasswordHashingService();

            var hash = hashingService.Hash(password);

            Assert.IsFalse(hashingService.Verify("wrong password", hash));

        }
    }
}
