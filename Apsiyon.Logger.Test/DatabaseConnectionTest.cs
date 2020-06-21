using Apsiyon.Logger.Helper;
using ApsiyonLogger.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Apsiyon.Logger.Test
{
    [TestFixture]
    public class DatabaseConnectionTest
    {
        [Test]
        public void Test()
        {
            Helper.ConfigurationHelper.Configuration();
            if (ConfigurationDto.Config.SqlServer.Active)
            {
                var serviceCollection = new ServiceCollection();
                serviceCollection.AddDbContext<DataContext>(a => a.UseSqlServer(ConfigurationDto.Config.SqlServer.ConnectionString, x => x.MigrationsHistoryTable(ConfigurationDto.Config.SqlServer.MigrationHistoryTableName).MigrationsAssembly("Apsiyon.Logger.Data.Migration")));
                
                using (var serviceScope = serviceCollection.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
                using (var dataContext = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    dataContext.Database.OpenConnection();
                }
            }
        }
    }
}