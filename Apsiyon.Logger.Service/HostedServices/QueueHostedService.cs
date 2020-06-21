using System;
using System.Threading;
using System.Threading.Tasks;
using Apsiyon.Logger.Interface.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Apsiyon.Logger.Service.HostedServices
{
    public class QueueHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public bool Cancel;

        public QueueHostedService(IServiceScopeFactory scopeFactory)
        {
            _serviceScopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            new Thread(Worker) { IsBackground = true, Name = "ApsiyonLogger_QueueHostedServiceThread" }.Start();

            return Task.CompletedTask;
        }

        private async void Worker()
        {
            while (true)
            {
                try
                {
                    if (Cancel) break;

                    var log = Helper.LogQueueHelper.Get();

                    if (log != null)
                    {
                        using (IServiceScope serviceScope = _serviceScopeFactory.CreateScope())
                        using (IUnitOfWork unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>())
                        {
                            await unitOfWork.LogInjectorService.Fire(log);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //TODO: Handle Exception
                }

                Thread.Sleep(500);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Cancel = true;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Cancel = true;
        }
    }
}