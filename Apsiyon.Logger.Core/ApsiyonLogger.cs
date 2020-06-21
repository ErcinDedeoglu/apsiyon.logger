using System;
using Apsiyon.Logger.Helper;
using Apsiyon.Logger.Interface.UnitOfWork;
using Apsiyon.Logger.Service.HostedServices;
using Apsiyon.Logger.Service.UnitOfWork;
using ApsiyonLogger.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apsiyon.Logger.Core
{
    public static class ApsiyonLogger
    {
        public static void UseApsiyonLogger(this IServiceCollection serviceCollection)
        {
            var configuration = serviceCollection.BuildServiceProvider().GetService<IConfiguration>();
            ConfigurationDto.Config = configuration.GetSection("ApsiyonLogger").Get<ConfigurationDto.Configuration>();

            if (ConfigurationDto.Config.SqlServer.Active)
            {
                serviceCollection.AddDbContext<DataContext>(a => a.UseSqlServer(ConfigurationDto.Config.SqlServer.ConnectionString, x => x.MigrationsHistoryTable(ConfigurationDto.Config.SqlServer.MigrationHistoryTableName).MigrationsAssembly("Apsiyon.Logger.Data.Migration")));
                
                using (var serviceScope = serviceCollection.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope())
                using (var dataContext = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    try
                    {
                        dataContext.Database.Migrate();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Apsiyon.Logger: Cannot reach to database server. Database server logging switching off.");
                        ConfigurationDto.Config.SqlServer.Active = false;
                    }
                }
            }

            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<Interface.ILogger, Service.Logger>();
            serviceCollection.AddHostedService<QueueHostedService>();
        }
    }
}