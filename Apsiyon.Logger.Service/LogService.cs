using Apsiyon.Logger.Data.Entity;
using Apsiyon.Logger.Interface;
using Apsiyon.Logger.Service.UnitOfWork;
using ApsiyonLogger.Data.Context;

namespace Apsiyon.Logger.Service
{
    public class LogService : Repository<Log>, ILogService
    {
        private readonly DataContext _dataContext;

        public LogService(DataContext dataContext) : base(dataContext)
        {
            _dataContext = dataContext;
        }
    }
}