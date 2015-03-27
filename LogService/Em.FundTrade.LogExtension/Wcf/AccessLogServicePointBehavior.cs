using Em.FundTrade.LogExtension.Log;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Em.FundTrade.LogExtension.Wcf
{
    public class AccessLogServicePointBehavior : IServiceBehavior
    {
        public AccessLogServicePointBehavior()
        {
            
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endPoint in serviceDescription.Endpoints)
            {
                endPoint.Behaviors.Add(new AccessLogEndPointBehavior());
            }
        }
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }
    }
}
