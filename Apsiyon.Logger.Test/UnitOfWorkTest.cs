using System.Threading.Tasks;
using Apsiyon.Logger.Helper;
using Apsiyon.Logger.Interface.UnitOfWork;
using Apsiyon.Logger.Service.UnitOfWork;
using ApsiyonLogger.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Apsiyon.Logger.Test
{
    [TestFixture]
    public class UnitOfWorkTest
    {
        [Test]
        public async Task Test()
        {
            Helper.ConfigurationHelper.Configuration();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<DataContext>(a => a.UseSqlServer(ConfigurationDto.Config.SqlServer.ConnectionString, x => x.MigrationsHistoryTable(ConfigurationDto.Config.SqlServer.MigrationHistoryTableName).MigrationsAssembly("Apsiyon.Logger.Data.Migration")));
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            
            using (var serviceScope = serviceCollection.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var unitOfWork = serviceScope.ServiceProvider.GetService<IUnitOfWork>())
            {
                Assert.IsNotNull(unitOfWork.ServiceScopeFactory);
                Assert.IsNotNull(unitOfWork.LogService);
                Assert.IsNotNull(unitOfWork.LogInjectorService);

                int recordCount = unitOfWork.Complete();
                Assert.AreEqual(0, recordCount);
                recordCount = await unitOfWork.CompleteAsync();
                Assert.AreEqual(0, recordCount);
            }
        }
    }
}