using Em.FundTrade.LogManager;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
namespace Em.FundTrade.LogExtension.Log
{
    public class LogManager
    {
        private static string[] excludeMethods;
        private static string[] methodWithoutResponseData;
        private static LogManagerLib logManager = LogManagerLib.Instance;
        private static bool isWriteLog;
        public static bool IsWriteLog
        {
            get
            {
                return LogManager.isWriteLog;
            }
        }
        static LogManager()
        {
            LogManager.excludeMethods = null;
            LogManager.methodWithoutResponseData = null;
            LogManager.ResetWriteLogSetting();
            Task.Factory.StartNew(delegate
            {
                while (true)
                {
                    System.Threading.Thread.Sleep(10000);
                    try
                    {
                        LogManager.ResetWriteLogSetting();
                    }
                    catch (System.Exception)
                    {
                    }
                }
            });
        }
        private static void ResetWriteLogSetting()
        {
            try
            {
                Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                string value = "false";
                if (configuration.AppSettings.Settings["ACCESSLOG_ENABLED"] != null)
                {
                    value = configuration.AppSettings.Settings["ACCESSLOG_ENABLED"].Value;
                }
                
                LogManager.isWriteLog = System.Convert.ToBoolean(value);
                string[] array = new string[0];
                if (configuration.AppSettings.Settings["ExcludeMethod"] != null)
                {
                    array = configuration.AppSettings.Settings["ExcludeMethod"].Value.Split(new char[]
				            {
					            ','
				            });
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = array[i].Trim(new char[]
					                {
						                '\r',
						                '\n',
						                '\t',
						                ' '
					                });
                    }
                }

                string[] array2 = new string[0];
                if (configuration.AppSettings.Settings["MethodWithoutResponseData"] != null)
                {
                    array2 =  configuration.AppSettings.Settings["MethodWithoutResponseData"].Value.Split(new char[]
				                {
					                ','
				                });
                    for (int i = 0; i < array2.Length; i++)
                    {
                        array2[i] = array2[i].Trim(new char[]
					                {
						                '\r',
						                '\n',
						                '\t',
						                ' '
					                });
                    }
                }

                LogManager.excludeMethods = array;
                LogManager.methodWithoutResponseData = array2;
            }
            catch (System.Exception ex)
            {
                LogManager.logManager.WriteErorrLog("ResetWriteLogFlag:" + ex.Message + ex.StackTrace);
                LogManager.isWriteLog = false;
            }
            System.Console.WriteLine("ResetWriteLogFlag = " + LogManager.isWriteLog);
        }
        public static void WriteAccessLog(AccessLog accessLog)
        {
            if (LogManager.isWriteLog)
            {
                //Task.Factory.StartNew(delegate(object data)
                //{
                    try
                    {
                        AccessLog accessLog2 = accessLog as AccessLog;
                        string[] array2 = LogManager.excludeMethods;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string text = array2[i];
                            if ((string.Concat(accessLog2.InterfaceName , "/" , accessLog2.MethodName)).ToLower().Equals(text.ToLower()))
                            {
                                return;
                            }
                        }
                        array2 = LogManager.methodWithoutResponseData;
                        for (int i = 0; i < array2.Length; i++)
                        {
                            string text = array2[i];
                            if ((string.Concat(accessLog2.InterfaceName , "/" , accessLog2.MethodName)).ToLower().Equals(text.ToLower()))
                            {
                                accessLog2.ResponseData = "";
                            }
                        }
                        accessLog2.TimeInterval = System.Convert.ToInt64((accessLog2.EndTime - accessLog2.StartTime).TotalMilliseconds);
                        LogServiceClient.WriteLog(accessLog2);
                    }
                    catch (System.Exception ex)
                    {
                        System.Console.WriteLine(ex.GetType().FullName + ex.Message);
                        LogManager.logManager.WriteErorrLog("WriteAccessLog:" + ex.Message + ex.StackTrace);
                    }
                //}, accessLog);
            }
        }
    }
}