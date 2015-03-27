using Em.FundTrade.LogManager;
using log4net;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Em.FundTrade.LogExtension.Log
{
    public class BaseAdapter
    {
        private LogManagerLib logManager = LogManagerLib.Instance;

        protected int commitedNum = 10000;
        protected virtual string CreatDBSql()
        {
            return "";
        }

        protected virtual bool IsNeedCreatDBSql()
        {
            Console.WriteLine("Base-IsNeedCreatDBSql");
            return true;
        }

        protected virtual void CreateDB()
        {
            string srcConnString = System.Configuration.ConfigurationManager.AppSettings["LOG_DB"];
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            string sql = CreatDBSql();
            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(srcConnString);
                connection.Open();
                MySql.Data.MySqlClient.MySqlCommand command = null;
                command = new MySql.Data.MySqlClient.MySqlCommand();
                command.Connection = connection;
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message + ee.StackTrace);
                logManager.WriteErorrLog(string.Concat("CreateDB:", ee.Message, ee.StackTrace));
                throw ee;
            }
            finally
            {
                connection.Close();
            }
        }

        protected virtual List<T> BaseRemoveLogFromFile<T>()
        {
            List<T> logs = new List<T>();
            if (!Directory.Exists(typeof(T).Name)) return logs;

            string[] files = Directory.GetFiles(typeof(T).Name, "*.log", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception ee)
                {
                    logManager.WriteErorrLog(string.Concat("BaseRemoveLogFromFile:", ee.Message, ee.StackTrace));
                    Console.WriteLine(file + "无法删除：" + ee.Message);
                }
            }
            return logs;
        }

        protected virtual List<T> BaseReadLogFromFile<T>()
        {
            List<T> logs = new List<T>();
            if (!Directory.Exists(typeof(T).Name)) return logs;

            string[] files = Directory.GetFiles(typeof(T).Name, "*.log", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                try
                {
                    List<T> logInFile = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(file));
                    logs.AddRange(logInFile);
                }
                catch (Exception ee)
                {
                    logManager.WriteErorrLog(string.Concat("BaseReadLogFromFile:", ee.Message, ee.StackTrace));
                    Console.WriteLine(file + "格式不对" + ee.Message);
                }
            }
            return logs;
        }

        protected virtual bool BaseWriteLogInFile(string logName,object data)
        {
            if (!Directory.Exists(logName))
            {
                Directory.CreateDirectory(logName);
            }
            File.WriteAllText(string.Concat(logName,"/", DateTime.Now.ToString("yyyyMMddhhMMss"), ".log"), JsonConvert.SerializeObject(data));
            return true;
        }

        protected virtual bool BaseWriteLogInDB<T>(Action afterCreateDB,
                                                   string insertSql,
                                                   List<T> logList,
                                                   Action<Exception> exceptionHappened,
                                                   out List<T> errorLogList) where T :BaseLog
        {
            int i = 0;
            int commitNum = 0;
            string srcConnString = System.Configuration.ConfigurationManager.AppSettings["LOG_DB"];
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlCommand command = null;
            errorLogList = new List<T>();

            try
            {
                if (IsNeedCreatDBSql())
                {
                    this.CreateDB();
                    afterCreateDB();
                }

                
                connection = new MySql.Data.MySqlClient.MySqlConnection(srcConnString);
                connection.Open();
                int max = logList.Count;
                StringBuilder sb = new StringBuilder();
                for (i = 0; i < max; i++)
                {
                    if(sb.Length == 0)
                        sb.Append(string.Format("({0})", logList[i].SqlConvert()));
                    else
                        sb.Append(string.Format(",({0})", logList[i].SqlConvert()));

                    if ((i + 1) % commitedNum == 0 || i + 1 == max)
                    {
                        if (i == 0 && i != max - 1)
                            continue;

                        Console.WriteLine("data commited");
                        command = new MySql.Data.MySqlClient.MySqlCommand();
                        command.Connection = connection;
                        sb.Insert(0,insertSql);
                        command.CommandText = sb.ToString();
                        command.ExecuteNonQuery();
                        commitNum = i;
                        sb = new StringBuilder();
                    }
                    //command = new MySql.Data.MySqlClient.MySqlCommand();
                    //command.Connection = connection;
                    //command.CommandText = insertSql;
                    //command.Transaction = tran;
                    //addParam(command, logList[i]);
                    //command.ExecuteNonQuery();

                    //if ((i + 1) % commitedNum == 0 || i + 1 == max)
                    //{
                    //    if (i == 0 && i != max - 1)
                    //        continue;

                    //    Console.WriteLine("data commited");
                    //    tran.Commit();
                    //    commitNum = i;
                    //    tran = connection.BeginTransaction();
                    //}
                }
            }
            catch (Exception ee)
            {
                exceptionHappened(ee);
                logManager.WriteErorrLog(string.Concat("BaseWriteLogInDB:", ee.Message, ee.StackTrace));
                Console.WriteLine(ee.Message + ee.StackTrace);
                if (commitNum<logList.Count-1)
                    errorLogList.AddRange(logList.GetRange(commitNum + 1, logList.Count - commitNum - 1));
                return false;
            }
            finally
            {
                connection.Close();
            }
            return true;
        }
    }
}
