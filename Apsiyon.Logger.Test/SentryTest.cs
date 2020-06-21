using NUnit.Framework;

namespace Apsiyon.Logger.Test
{
    [TestFixture]
    public class SentryTest
    {
        [Test]
        public void Test()
        {
            Helper.ConfigurationHelper.Configuration();
            var sentryHelper = new Logger.Helper.SentryHelper(Logger.Helper.ConfigurationDto.Config.Sentry.DSN);
            string sentryId = sentryHelper.Capture("Apsiyon.Logger UNIT TEST").ToString();

            Assert.IsNotEmpty(sentryId);
        }
    }
}