using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Em.FundTrade.LogExtension.Log
{
    public class AccessLogAdapter :BaseAdapter
    {
        public AccessLogAdapter()
        {
            var batchNums = System.Configuration.ConfigurationManager.AppSettings["AddBatchNums"];
            if (batchNums != null)
                SetCommitedNum(Convert.ToInt32(batchNums));
        }

        private static DateTime currentDateTime = new DateTime();
        private static bool isNeedCreateDB = true;
        protected override string CreatDBSql()
        {
            return string.Format(@"CREATE TABLE if not exists accesslog{0}(
                    ID bigint(20) auto_increment primary key, 
                    UUID varchar(50), 
                    ClientIP varchar(50), 
                    ServerIP varchar(50), 
                    InterfaceName varchar(100), 
                    MethodName varchar(100), 
                    StartTime timestamp, 
                    EndTime timestamp, 
                    TimeInterval bigint(20),
                    RequestData longtext, 
                    ResponseData longtext) DEFAULT CHARSET=utf8", currentDateTime.ToString("yyyyMMddHH"));
        }

        protected void SetCommitedNum(int CommitedNum)
        {
            this.commitedNum = CommitedNum;
        }

        protected override bool IsNeedCreatDBSql()
        {
            Console.WriteLine("AccessLogAdapter-IsNeedCreatDBSql");
            DateTime now = DateTime.Now;
            if (!currentDateTime.ToString("yyyyMMddHH").Equals(now.ToString("yyyyMMddHH")))
            {
                currentDateTime = now;
                Console.WriteLine("AccessLogAdapter-IsNeedCreatDBSql-true");
                return true;
            }
            else
            {
                Console.WriteLine("AccessLogAdapter-IsNeedCreatDBSqlByisNeedCreateDB-" + isNeedCreateDB);
                return isNeedCreateDB;
            }
                
        }

        private List<AccessLog> logList = new List<AccessLog>();
        public void Put(BaseLog input)
        {
            if (input is AccessLog)
            {
                AccessLog log = input as AccessLog;
                log.RequestData = ReadData(log.RequestData);
                log.ResponseData = ReadData(log.ResponseData);
                logList.Add(log);
            }
        }
        public void Put(List<BaseLog> input)
        {
            input.ForEach(p => this.Put(p));
        }

        public void Clear()
        {
            this.logList = new List<AccessLog>();
        }

        private string ReadData(string content)
        {
            try
            {
                return content.Substring(content.IndexOf("<s:Body>") + 8, content.IndexOf("</s:Body>") - content.IndexOf("<s:Body>") - 8).Trim();
            }
            catch
            {
                return content;
            }
        }

        public List<AccessLog> ReadLogFromFile()
        {
            return BaseReadLogFromFile<AccessLog>();
        }
        public List<AccessLog> RemoveLogFromFile()
        {
            return BaseRemoveLogFromFile<AccessLog>();
        }

        public bool WriteLogInFile() 
        {
            return BaseWriteLogInFile("accesslog", logList);
        }

        public bool WriteLogInDB()
        {
            //string sql = string.Format(@"INSERT INTO AccessLog{0} (UUID,ClientIP,ServerIP,InterfaceName,MethodName,StartTime,EndTime,TimeInterval,RequestData,ResponseData)
            //                                    VALUES (?UUID, ?ClientIP, ?ServerIP,?MethodName,?StartTime,?EndTime,?TimeInterval,?RequestData,?ResponseData)", DateTime.Now.ToString("yyyyMMdd"));
            string sql = string.Format(@"INSERT INTO accesslog{0} (UUID,ClientIP,ServerIP,InterfaceName,MethodName,StartTime,EndTime,TimeInterval,RequestData,ResponseData)
                                                VALUES ", DateTime.Now.ToString("yyyyMMddHH"));

            List<AccessLog> unCommitedLog = new List<AccessLog>();
            var result = this.BaseWriteLogInDB<AccessLog>(
                () =>
                {
                    isNeedCreateDB = false;
                }, sql, logList,
                (exception) =>
                {
                    isNeedCreateDB = false;
                }, out unCommitedLog);
            if (!result)
                this.logList = unCommitedLog;
            return result;
        }
    }
}
