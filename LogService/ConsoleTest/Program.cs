using Em.FundTrade.LogExtension.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        public static Func<T, U> MakeDelegate<T,U>(Object client,string methodName)
        {
            MethodInfo mi = client.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            Func<T, U> result = (input) =>
            {
                var data = mi.Invoke(client, new object[] { input });
                return (U)data;
            };
            return result;
        }
        private static void ServerDebug()
        {
            //Task.Factory.StartNew(() =>
            //{
            //    //LogService.LogManager.Maintenance();
            //    //log4net.Config.XmlConfigurator.Configure();
            //    //ServiceHost myServiceHost = new ServiceHost(typeof(LogService.LogManager));
            //    //myServiceHost.Open();

            //    try
            //    {
            //        ServiceHost myServiceHost1 = new ServiceHost(typeof(GetIdentity));
            //        myServiceHost1.Open();
            //    }
            //    catch (Exception ee)
            //    {
            //        Console.WriteLine(ee.Message);
            //    }
            //});
            //Console.WriteLine("ServiceStarted");
            //Thread.Sleep(5000);

            var proxyPool = WCFProxyPool.ChannelFactoryPool<IGetIdentity>.Instance;
            proxyPool.Size = 10;
            proxyPool.ServiceURI = "net.tcp://172.16.86.58:8067/WCFService/";
            proxyPool.Init();

            try
            {
                Task.Factory.StartNew(() =>
                {
                    int num = 10000;
                    while (num > 0)
                    {
                        num--;
                        IGetIdentity client = proxyPool.GetClient();
                        var function = MakeDelegate<string, string>(client, "Get");
                        var result = function("aaa");
                        proxyPool.ReleaseClient(client);
                        //Thread.Sleep(1000);
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    int num = 10000;
                    while (num > 0)
                    {
                        num--;
                        IGetIdentity client = proxyPool.GetClient();
                        var function = MakeDelegate<string, string>(client, "Get");
                        var result = function("bbb");
                        proxyPool.ReleaseClient(client);
                        //Thread.Sleep(1000);
                    }
                });

                Task.Factory.StartNew(() =>
                {
                    int num = 10000;
                    while (num > 0)
                    {
                        num--;
                        IGetIdentity client = proxyPool.GetClient();
                        var function = MakeDelegate<string, string>(client, "Get");
                        var result = function("ccc");
                        proxyPool.ReleaseClient(client);
                        //Thread.Sleep(1000);
                    }
                });

                //IGetIdentity client1 = new GetIdentity();
                //var function1 = MakeDelegate<string, string>(client1, "Get");
                //var result1 = function1("aaa");
                

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }

            //AccessLog log = new AccessLog()
            //{
            //    UUID = "test",
            //    ClientIP = "127.0.0.1",
            //    ServerIP = "127.0.0.2",
            //    InterfaceName = "interface",
            //    MethodName = "testMethod",
            //    StartTime = DateTime.Now,
            //    EndTime = DateTime.Now,
            //    TimeInterval = 100,
            //    RequestData = "test",
            //    ResponseData = "teset"
            //};
            //AccessLog log1 = new AccessLog()
            //{
            //    UUID = "test",
            //    ClientIP = "127.0.0.1",
            //    ServerIP = "127.0.0.2",
            //    InterfaceName = "interface",
            //    MethodName = "testMethod",
            //    StartTime = DateTime.Now,
            //    EndTime = DateTime.Now,
            //    TimeInterval = 100,
            //    RequestData = "test我们",
            //    ResponseData = "teset我们"
            //};

            //LogServiceClient.WriteLog(log);
            //log1.UUID = "1";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "2";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "3";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "4";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "5";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "6";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "7";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "8";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "9";
            //LogServiceClient.WriteLog(log1);
            //log1.UUID = "10";
            //LogServiceClient.WriteLog(log1);

            Console.WriteLine("process all stop");
            Console.ReadLine();
        }

        static void Main(string[] args)
        {
            ServerDebug();
        }
    }
}
