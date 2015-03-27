using Em.FundTrade.LogManager;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCFProxyPool;

namespace Em.FundTrade.LogExtension.Log
{
    public class LogServiceClient
    {
        private static LogManagerLib logManager = LogManagerLib.Instance;

        private static ConcurrentQueue<AccessLog> logQuery = new ConcurrentQueue<AccessLog>();
        static LogServiceClient()
        {
            try
            {
                proxyPool = WCFProxyPool.ChannelFactoryPool<ILogManager>.Instance;
                proxyPool.Size = 10;
                proxyPool.ServiceURI = System.Configuration.ConfigurationManager.AppSettings["LOG_SERVICE"];
                proxyPool.Init();
            }
            catch(Exception e)
            {
                logManager.WriteErorrLog(string.Concat("LogServiceClient:", e.Message, e.StackTrace));
            }
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Console.WriteLine("while true");
                    AccessLog log = null;
                    ILogManager client = null;
                    List<AccessLog> logs = null;
                    try
                    {
                        client = proxyPool.GetClient();
                        int num = 1;
                        logs = new List<AccessLog>();
                        while (true)
                        {
                            if (logQuery.TryDequeue(out log))
                            {
                                logs.Add(log);
                                if (num % 100 == 0)
                                {
                                    num = 1;
                                    Console.WriteLine("send data"+logs.Count);
                                    client.WriteAccessLogs(logs);
                                    logs = new List<AccessLog>(); 
                                }
                                else
                                {
                                    num++;
                                }
                            }
                            else
                            {
                                num = 1;
                                Console.WriteLine("send data" + logs.Count);
                                if (logs.Count > 0)
                                {
                                    client.WriteAccessLogs(logs);
                                    logs = new List<AccessLog>(); 
                                }
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (logs != null)
                        {
                            logs.ForEach(p => logQuery.Enqueue(p));
                            logs = null;
                        }
                        logManager.WriteErorrLog(string.Concat("TaskWriteLog:", e.Message, e.StackTrace));
                        break;
                    }
                    finally
                    {
                        proxyPool.ReleaseClient(client);
                    }
                    Thread.Sleep(5000);
                }
            });
            
        }
        static WCFProxyPool.ChannelFactoryPool<ILogManager> proxyPool = null;
        public static void WriteLog(AccessLog log) 
        {
            try
            {
                logQuery.Enqueue(log);
            }
            catch (Exception e)
            {
                Console.WriteLine("logQuery Enqueue error");
                logManager.WriteErorrLog(string.Concat("WriteLog:", e.Message, e.StackTrace));
            }
            finally 
            {
            }
            
        }
    }
}
