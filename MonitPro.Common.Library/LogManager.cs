using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace MonitPro.Common.Library
{
    public sealed class LogManager
    {
        private static volatile LogManager instance;
        private static object syncRoot = new Object();

        private LogManager() { }

        public static LogManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new LogManager();
                    }
                }

                return instance;
            }
        }


        public void Error(Exception objException)
        {
            WriteInLog(string.Format("{0} :{1}", DateTime.Now, "Error Message"+objException.Message+"\nStackTrace :" +objException.StackTrace));
         }

        public  void Info(string infoMessage)
        {
            WriteInLog(string.Format("{0} :Info - {1}", DateTime.Now, infoMessage));
        }
 
        private void WriteInLog(string errorMessage)
        {
           
            if (AppConfig.IsLoggingEnabled)
            {
                string configFilePath = string.Format("{0}/DailyLog_{1}.txt", AppConfig.LogFilePath, "" + DateTime.Now.ToString("MMddyyyy"));

                if (!File.Exists(configFilePath))
                {
                    using (StreamWriter sw = File.CreateText(configFilePath))
                    {
                        sw.WriteLine(errorMessage);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(configFilePath))
                    {
                        sw.WriteLine(errorMessage);
                    }
                }
            }
        }

    }

}
