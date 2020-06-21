using System;
using System.Collections.Generic;

namespace Apsiyon.Logger.Helper
{
    public class LogQueueHelper
    {
        public static Queue<LogDto> Logs = new Queue<LogDto>();
        private static readonly object Locker = new object();

        public class LogDto
        {
            public string Message { get; set; }
            public object Object { get; set; }
            public string ObjectType { get; set; }
            public DateTime CreateDate { get; set; }
        }

        public static void Add(string message, object obj)
        {
            lock (Locker)
            {
                Logs.Enqueue(new LogDto()
                {
                    Message = message,
                    Object = obj,
                    ObjectType = obj?.GetType().FullName,
                    CreateDate = DateTime.UtcNow
                });
            }
        }

        public static LogDto Get()
        {
            LogDto result = null;

            lock (Locker)
            {
                if (Logs.Count > 0) result = Logs.Dequeue();
            }

            return result;
        }
    }
}