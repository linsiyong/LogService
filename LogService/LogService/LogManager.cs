using Em.FundTrade.LogExtension.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LogService
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class LogManager :ILogManager
    {
        public void WriteAccessLogs(List<AccessLog> logs)
        {
            //test123
            foreach (var log in logs)
            {
                logQuery.Enqueue(log);
            }
        }

        public void WriteAccessLog(AccessLog log)
        {
            logQuery.Enqueue(log);
        }

        private static ConcurrentQueue<BaseLog> logQuery = new ConcurrentQueue<BaseLog>();
        public static void WatiStop()
        {
            needStop = true;
            Task.WaitAll(ts);
            if (logQuery != null)
            {
                BaseLog log = null;
                Console.WriteLine(logQuery.Count);
                AccessLogAdapter logAdapter = new AccessLogAdapter();
                while (logQuery.TryDequeue(out log))
                {
                    logAdapter.Put(log);
                }
                if (!logAdapter.WriteLogInDB())
                {
                    logAdapter.WriteLogInFile();
                }
            }
        }

        private static bool needStop=false;

        private static Task ts = null;

        public static void Maintenance()
        {
            ts = Task.Factory.StartNew(() =>
            {
                while (!needStop)
                {
                    Thread.Sleep(10000);
                    var tempLogQuery = logQuery;
                    logQuery = new ConcurrentQueue<BaseLog>();

                    BaseLog log = null;
                    AccessLogAdapter logAdapter = new AccessLogAdapter();

                    while (tempLogQuery.TryDequeue(out log))
                    {
                        logAdapter.Put(log);
                    }

                    if (!logAdapter.WriteLogInDB())
                    {
                        logAdapter.WriteLogInFile();
                    }
                    else
                    {
                        logAdapter.Clear();
                        var list = logAdapter.ReadLogFromFile();
                        if (list != null && list.Count>0)
                        {
                            list.ForEach(p => logAdapter.Put(p));
                            if (logAdapter.WriteLogInDB())
                            {
                                logAdapter.RemoveLogFromFile();
                            }
                        }
                    }

                }
            });
        }

    }
}
