using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ClsLibBKLogs
{
    public class Logger
    {
        public static Environment.SpecialFolder folderType = Environment.SpecialFolder.ApplicationData;

        public static bool enableErrorLogging = true;
               
        public static string appName;

        public static bool LogError(string fileNameWithExtension, string msgText, bool append)
        {
            if (!enableErrorLogging) return true;

            bool result = false;          

            try
            {
                string folderpath = Environment.GetFolderPath(folderType);

                string errorLogDir = folderpath + @"\" + appName + "ErrorLogs";

                if (!Directory.Exists(errorLogDir))
                {
                    Directory.CreateDirectory(errorLogDir);
                }

                string filepath = errorLogDir + @"\" + fileNameWithExtension;

                StreamWriter sw = new StreamWriter(filepath,append);
                sw.NewLine = "\r\n";

                sw.WriteLine("----------------------// " + DateTime.Now.ToString() + " //-----------------------------");
                sw.WriteLine(msgText);

                sw.Close();

                result = true;
                
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static bool LogError(string msgText)
        {
            if (!enableErrorLogging) return true;

            bool result = false;
            bool append = true;

            try
            {
                string folderpath = Environment.GetFolderPath(folderType);

                string errorLogDir = folderpath + @"\" + appName + "ErrorLogs";               

                if (!Directory.Exists(errorLogDir))
                {
                    Directory.CreateDirectory(errorLogDir);
                }

                string filepath = errorLogDir + @"\ErrorLog_" + DateTime.Today.DayOfWeek.ToString() + ".txt";

                FileInfo finfo = new FileInfo(filepath);
                if (finfo.Exists && finfo.CreationTime.Date < DateTime.Today.Date)
                {
                    finfo.Delete();                    
                }
                
                StreamWriter sw = new StreamWriter(filepath, append);

                sw.WriteLine("----------------------// " + DateTime.Now.ToString() + " //-----------------------------");
                sw.WriteLine(msgText);

                sw.Close();

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static bool LogError(string folderPath,string fileNameWithExtension, string msgText, bool append)
        {
            if (!enableErrorLogging) return true;

            bool result = false;
            try
            {
               // string folderpath = Environment.GetFolderPath(folderType);

                string errorLogDir = folderPath + @"\" + appName + "ErrorLogs";

                if (!Directory.Exists(errorLogDir))
                {
                    Directory.CreateDirectory(errorLogDir);
                }

                string filepath = errorLogDir + @"\" + fileNameWithExtension;

                StreamWriter sw = new StreamWriter(filepath, append);

                sw.WriteLine("----------------------// " + DateTime.Now.ToString() + " //-----------------------------");
                sw.WriteLine(msgText);

                sw.Close();

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static bool LogError(string folderPath,string msgText)
        {
            if (!enableErrorLogging) return true;

            bool result = false;
            bool append = true;

            try
            {
               // string folderpath = Environment.GetFolderPath(folderType);

                string errorLogDir = folderPath + @"\" + appName + "ErrorLogs";

                if (!Directory.Exists(errorLogDir))
                {
                    Directory.CreateDirectory(errorLogDir);
                }

                string filepath = errorLogDir + @"\ErrorLog_" + DateTime.Today.DayOfWeek.ToString() + ".txt";

                FileInfo finfo = new FileInfo(filepath);
                if (finfo.Exists && finfo.CreationTime.Date < DateTime.Today.Date)
                {
                    finfo.Delete();
                }

                StreamWriter sw = new StreamWriter(filepath, append);

                sw.WriteLine("----------------------// " + DateTime.Now.ToString() + " //-----------------------------");
                sw.WriteLine(msgText);

                sw.Close();

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

    }
}
