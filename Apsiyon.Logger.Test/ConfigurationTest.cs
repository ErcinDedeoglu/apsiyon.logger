using NUnit.Framework;

namespace Apsiyon.Logger.Test
{
    [TestFixture]
    public class ConfigurationTest
    {
        [Test]
        public void Test()
        {
            Helper.ConfigurationHelper.Configuration();

            if (Logger.Helper.ConfigurationDto.Config.File.Active)
            {
                Assert.IsNotEmpty(Logger.Helper.ConfigurationDto.Config.File.LogName);
                Assert.IsNotEmpty(Logger.Helper.ConfigurationDto.Config.File.Path);
            }
            
            if (Logger.Helper.ConfigurationDto.Config.Sentry.Active)
            {
                Assert.IsNotEmpty(Logger.Helper.ConfigurationDto.Config.Sentry.DSN);
            }
            
            if (Logger.Helper.ConfigurationDto.Config.SqlServer.Active)
            {
                Assert.IsNotEmpty(Logger.Helper.ConfigurationDto.Config.SqlServer.ConnectionString);
                Assert.IsNotEmpty(Logger.Helper.ConfigurationDto.Config.SqlServer.MigrationHistoryTableName);
            }
        }
    }
}