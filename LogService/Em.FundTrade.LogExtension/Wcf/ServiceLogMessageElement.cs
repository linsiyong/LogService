using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

namespace Em.FundTrade.LogExtension.Wcf
{
    public class ServiceLogMessageElement : BehaviorExtensionElement
    {
        public ServiceLogMessageElement()
        {
        }

        public override Type BehaviorType
        {
            get
            {
                return typeof(AccessLogServicePointBehavior);
            }
        }

        protected override object CreateBehavior()
        {
            return new AccessLogServicePointBehavior();
        }
    }
}
