using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Em.FundTrade.LogExtension.Log
{
    public class AccessLog : BaseLog
    {
        public string UUID{get;set;}
        public string ClientIP { get; set; }
        public string ServerIP { get; set; }
        public string InterfaceName { get; set; }
        public string MethodName{get;set;}
        public DateTime StartTime{get;set;}
        public DateTime EndTime{get;set;}
        public long TimeInterval{get;set;}
        public string RequestData { get; set; }
        public string ResponseData { get; set; }
        public override string SqlConvert()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("'").Append(UUID).Append("',");
            sb.Append("'").Append(ClientIP).Append("',");
            sb.Append("'").Append(ServerIP).Append("',");
            sb.Append("'").Append(InterfaceName).Append("',");
            sb.Append("'").Append(MethodName).Append("',");
            sb.Append("'").Append(StartTime.ToString("yyyy/MM/dd HH:mm:ss.fff")).Append("',");
            sb.Append("'").Append(EndTime.ToString("yyyy/MM/dd HH:mm:ss.fff")).Append("',");
            sb.Append("'").Append(TimeInterval.ToString()).Append("',");
            sb.Append("'").Append(RequestData.Replace("'", "\'")).Append("',");
            sb.Append("'").Append(ResponseData.Replace("'", "\'")).Append("'");
            return sb.ToString();
        }
    }
}
