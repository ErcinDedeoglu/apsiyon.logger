using System.Threading.Tasks;
using Apsiyon.Logger.Helper;

namespace Apsiyon.Logger.Interface
{
    public interface ILogInjectorService
    {
        Task Fire(LogQueueHelper.LogDto log);
    }
}