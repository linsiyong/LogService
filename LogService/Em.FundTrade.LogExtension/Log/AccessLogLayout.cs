using log4net.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Em.FundTrade.LogExtension.Log
{
    public class AccessLogLayout : PatternLayout
    {
        public AccessLogLayout()
        {
            this.AddConverter("property", typeof(AccessLogMessagePatternConverter));
        }
    }
}
