using Apsiyon.Logger.Interface;

namespace Apsiyon.Logger.Service
{
    public class Logger : ILogger
    {
        public void Add(string message, object obj)
        {
            Helper.LogQueueHelper.Add(message, obj);
        }
    }
}