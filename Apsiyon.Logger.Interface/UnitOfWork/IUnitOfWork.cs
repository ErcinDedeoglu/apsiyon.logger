using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Apsiyon.Logger.Interface.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILogService LogService { get; set; }
        ILogInjectorService LogInjectorService { get; set; }
        IServiceScopeFactory ServiceScopeFactory { get; set; }

        int Complete();
        Task<int> CompleteAsync();
    }
}