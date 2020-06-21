using System;
using System.Linq;
using System.Threading.Tasks;
using Apsiyon.Logger.Data.Entity;
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
    public class SQLServerTest
    {
        [Test]
        public async Task Test()
        {
            long ticks = DateTime.UtcNow.Ticks;
            Helper.ConfigurationHelper.Configuration();
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<DataContext>(a => a.UseSqlServer(ConfigurationDto.Config.SqlServer.ConnectionString, x => x.MigrationsHistoryTable(ConfigurationDto.Config.SqlServer.MigrationHistoryTableName).MigrationsAssembly("Apsiyon.Logger.Data.Migration")));
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

            LogQueueHelper.LogDto log = new LogQueueHelper.LogDto()
            {
                ObjectType = DateTime.UtcNow.GetType().FullName,
                Object = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow,
                Message = "Apsiyon.Logger UNIT TEST - " + ticks
            };

            using (var serviceScope = serviceCollection.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var unitOfWork = serviceScope.ServiceProvider.GetService<IUnitOfWork>())
            {
                unitOfWork.LogService.InsertAsync(new Log()
                {
                    CreateDate = log.CreateDate,
                    Message = log.Message,
                    Object = ObjectHelper.ObjectToByteArray(log.Object),
                    ObjectType = log.ObjectType
                });

                await unitOfWork.CompleteAsync();

                var record = unitOfWork.LogService.Query().FirstOrDefault(a => a.Message.Contains(ticks.ToString()));

                Assert.IsNotNull(record);
            }
        }
    }
}