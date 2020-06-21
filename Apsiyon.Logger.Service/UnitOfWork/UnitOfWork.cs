using System.Threading.Tasks;
using Apsiyon.Logger.Interface;
using Apsiyon.Logger.Interface.UnitOfWork;
using ApsiyonLogger.Data.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Apsiyon.Logger.Service.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;
        public UnitOfWork(IServiceScopeFactory serviceScopeFactory)
        {
            LogInjectorService = new LogInjectorService(serviceScopeFactory);
            ServiceScopeFactory = serviceScopeFactory;
        }

        public UnitOfWork(DataContext dbContext, IServiceScopeFactory serviceScopeFactory)
        {
            _dataContext = dbContext;
            LogService = new LogService(_dataContext);
            LogInjectorService = new LogInjectorService(serviceScopeFactory);
            ServiceScopeFactory = serviceScopeFactory;
        }

        public ILogService LogService { get; set; }
        public ILogInjectorService LogInjectorService { get; set; }
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        public int Complete() =>
            _dataContext.SaveChanges();

        public Task<int> CompleteAsync() =>
            _dataContext.SaveChangesAsync();

        public void Dispose() =>
            _dataContext?.Dispose();
    }
}