using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Em.FundTrade.LogExtension.Log
{
    [ServiceContract]
    public interface ILogManager
    {
        [OperationContract]
        void WriteAccessLog(AccessLog log);

        [OperationContract]
        void WriteAccessLogs(List<AccessLog> logs);
    }
}
