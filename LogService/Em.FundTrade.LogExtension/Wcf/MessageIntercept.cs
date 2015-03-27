using Em.FundTrade.LogExtension.Log;
using Em.FundTrade.LogManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;

namespace Em.FundTrade.LogExtension.Wcf
{
    /// <summary>  
    /// 消息拦截器【实现客户端和服务端的消息拦截】  
    /// </summary>  
    public class MessageIntercept : IClientMessageInspector, IDispatchMessageInspector
    {
        private LogManagerLib logManager = LogManagerLib.Instance;

        private ConcurrentDictionary <string, AccessLog> logs = new ConcurrentDictionary<string, AccessLog>();
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            if (!Log.LogManager.IsWriteLog) return;
            try
            {
                string uuid = reply.Headers.RelatesTo.ToString();
                AccessLog log = null;
                if (uuid != null && logs.TryRemove(uuid, out log))
                {
                    if (log != null)
                    {
                        log.EndTime = DateTime.Now;
                        log.ResponseData = reply.ToString();
                        Log.LogManager.WriteAccessLog(log);
                    }
                }
            }
            catch (Exception e)
            {
                logManager.WriteErorrLog(string.Concat("AfterReceiveReply:", e.Message, e.StackTrace));
            }
            //Console.WriteLine("客户端接收到的回复:{0}\n", reply);
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            if (!Log.LogManager.IsWriteLog) return null;
            try
            {
                string uuid = request.Headers.MessageId.ToString();
                if (!string.IsNullOrEmpty(uuid))
                {
                    AccessLog log = new AccessLog();
                    log.UUID = uuid;
                    log.StartTime = DateTime.Now;
                    log.ClientIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                    string action = request.Headers.Action;
                    try
                    {
                        string method = action.Substring(action.LastIndexOf("/") + 1);
                        string last = action.Substring(0, action.LastIndexOf("/"));
                        string interfaceName = last.Substring(last.LastIndexOf("/") + 1);
                        log.InterfaceName = interfaceName;
                        log.MethodName = method;
                    }
                    catch
                    {
                        log.InterfaceName = action;
                        log.MethodName = action;
                    }

                    log.RequestData = request.ToString();
                    logs.TryAdd(uuid, log);
                }
            }
            catch (Exception e)
            {
                logManager.WriteErorrLog(string.Concat("BeforeSendRequest:", e.Message, e.StackTrace));
            }
            return null;
        }

        public string ClientIp()
        {
            OperationContext context = OperationContext.Current;
            MessageProperties properties = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            return endpoint.Address;
        }

        

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            if (!Log.LogManager.IsWriteLog) return null;
            try
            {
                string uuid = request.Headers.MessageId.ToString();
                if (!string.IsNullOrEmpty(uuid))
                {
                    AccessLog log = new AccessLog();
                    log.UUID = uuid;
                    log.StartTime = DateTime.Now;
                    log.ClientIP = ClientIp();
                    log.ServerIP = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
                    string action = request.Headers.Action;
                    try
                    {
                        string method = action.Substring(action.LastIndexOf("/") + 1);
                        string last = action.Substring(0, action.LastIndexOf("/"));
                        string interfaceName = last.Substring(last.LastIndexOf("/") + 1);
                        log.InterfaceName = interfaceName;
                        log.MethodName = method;
                    }
                    catch
                    {
                        log.InterfaceName = action;
                        log.MethodName = action;
                    }
                    //log.MethodName = request.Headers.Action;
                    log.RequestData = request.ToString();
                    logs.TryAdd(uuid, log);
                }
            }
            catch (Exception e)
            {
                logManager.WriteErorrLog(string.Concat("AfterReceiveRequest:", e.Message, e.StackTrace));
            }
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            if (!Log.LogManager.IsWriteLog) return;
            try
            {
                string uuid = reply.Headers.RelatesTo.ToString();
                AccessLog log = null;
                if (uuid != null && logs.TryRemove(uuid, out log))
                {
                    if (log != null)
                    {
                        log.EndTime = DateTime.Now;
                        log.ResponseData = reply.ToString();
                        Log.LogManager.WriteAccessLog(log);
                    }
                }
                else
                {
                    Console.WriteLine("remove failed");

                }
            }
            catch (Exception e)
            {
                logManager.WriteErorrLog(string.Concat("BeforeSendReply:", e.Message, e.StackTrace));
            }
        }
    }
}
