using Em.FundTrade.LogExtension.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Em.FundTrade.LogExtension.Wcf
{
    /// <summary>  
    /// 将自定义的消息拦截器(MessageIntercept)插入到终结点行为  
    /// </summary>  
    public class AccessLogEndPointBehavior : IEndpointBehavior
    {
        public AccessLogEndPointBehavior()
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //植入"窃听器"客户端  
            clientRuntime.MessageInspectors.Add(new MessageIntercept());
        }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //植入"窃听器"服务端  
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new MessageIntercept());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            //不需要  
        }
    }
}
