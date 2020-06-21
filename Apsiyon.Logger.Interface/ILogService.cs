using Apsiyon.Logger.Data.Entity;
using Apsiyon.Logger.Interface.UnitOfWork;

namespace Apsiyon.Logger.Interface
{
    public interface ILogService : IRepository<Log>
    {
    }
}