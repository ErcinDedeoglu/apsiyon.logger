using System;
using System.Threading.Tasks;
using Apsiyon.Logger.Data.Entity;
using Apsiyon.Logger.Interface;
using Apsiyon.Logger.Interface.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Apsiyon.Logger.Service
{
    public class LogInjectorService : ILogInjectorService
    {
        private SQLServer SqlServer { get; }

        public LogInjectorService(IServiceScopeFactory serviceScopeFactory)
        {
            SqlServer = new SQLServer(Helper.ConfigurationDto.Config.SqlServer.Active, serviceScopeFactory);
            var file = new File(Helper.ConfigurationDto.Config.File.Active);
            var sentry = new Sentry(Helper.ConfigurationDto.Config.Sentry.Active);
            SqlServer.SetNext(file);
            file.SetNext(sentry);
        }
        
        public async Task Fire(Helper.LogQueueHelper.LogDto log) =>
            await Task.Run(() => SqlServer.Inject(log));

        private class SQLServer : Base
        {
            public SQLServer(bool active, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
            {
                Active = active;
            }

            public override async Task<bool> Insert(Helper.LogQueueHelper.LogDto log)
            {
                try
                {
                    using (var serviceScope = ServiceScopeFactory.CreateScope())
                    using (var unitOfWork = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>())
                    {
                        unitOfWork.LogService.InsertAsync(new Log()
                        {
                            CreateDate = log.CreateDate,
                            Message = log.Message,
                            Object = Helper.ObjectHelper.ObjectToByteArray(log.Object),
                            ObjectType = log.ObjectType
                        });

                        await unitOfWork.CompleteAsync();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    //TODO: Handle exception
                }

                return false;
            }
        }

        private class File : Base
        {
            public File(bool active)
            {
                Active = active;
            }

            public override async Task<bool> Insert(Helper.LogQueueHelper.LogDto log)
            {
                try
                {
                    string path = Helper.PathHelper.CreatePath();
                
                    Helper.FileHelper.CreateLogFile(path, log);
                    
                    return true;
                }
                catch (Exception ex)
                {
                    //TODO: Handle exception
                }

                return false;
            }
        }

        private class Sentry : Base
        {
            public Sentry(bool active)
            {
                Active = active;
            }

            public override async Task<bool> Insert(Helper.LogQueueHelper.LogDto log)
            {
                try
                {
                    var sentry = new Helper.SentryHelper(Helper.ConfigurationDto.Config.Sentry.DSN);
                    sentry.Capture(JsonConvert.SerializeObject(log));

                    return true;
                }
                catch (Exception ex)
                {
                    //TODO: Handle exception
                }

                return false;
            }
        }

        private abstract class Base
        {
            protected readonly IServiceScopeFactory ServiceScopeFactory;

            protected Base(IServiceScopeFactory serviceScopeFactory = null)
            {
                if (serviceScopeFactory != null) ServiceScopeFactory = serviceScopeFactory;
                
            }

            private Base _mSuccessor;
            protected bool Active;

            public void SetNext(Base account)
            {
                _mSuccessor = account;
            }

            public async void Inject(Helper.LogQueueHelper.LogDto log)
            {
                if (Active && await Insert(log))
                {
                    //log inserted
                    #if DEBUG
                    Console.WriteLine("Log: " + log.Message);
                    #endif
                }
                else if (_mSuccessor != null)
                {
                    _mSuccessor.Inject(log);
                }
                else
                {
                    Console.WriteLine("None of the logging targets work properly");
                }
            }

            public virtual async Task<bool> Insert(Helper.LogQueueHelper.LogDto log)
            {
                return false;
            }
        }
    }
}