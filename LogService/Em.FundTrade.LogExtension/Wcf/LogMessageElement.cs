using Em.FundTrade.LogExtension.Wcf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace Em.FundTrade.LogExtension.Wcf
{
    public class LogMessageElement : BehaviorExtensionElement
    {
        public LogMessageElement()
        {
        }

        public override Type BehaviorType
        {
            get 
            {
                return typeof(AccessLogEndPointBehavior); 
            }
        }

        protected override object CreateBehavior()
        {
            
            return new AccessLogEndPointBehavior();
            
        }
    }

   
}
