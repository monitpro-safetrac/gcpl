using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
//using MonitPro.Models;
using System.Web;
namespace MonitPro.Common.Library
{
    public class AppConfig
    {
        public static string ConnectionString
        {
            get
            { 
                return ConfigurationManager.ConnectionStrings["dbConnectionString"].ConnectionString;
            }

        }
        public static string LogFilePath
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFilePath"];
            }
        }

        public static bool IsLoggingEnabled
        {
            get
            { 
                if (ConfigurationManager.AppSettings["EnableLog"] == "true")
                    return true;
                else
                    return false;
            }
        }


       
    }
 
}


   
