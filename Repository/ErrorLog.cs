using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ATL_UPLOAD.Repository
{
    public class ErrorLog
    {
        public static void WriteException(string error, string Method)
        {
            string filelocation = AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog\\" + DateTime.Now.ToString("ddMMyyy") + "_ErrorLog.txt";

            if (!Directory.Exists(Path.GetDirectoryName(filelocation)))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog\\");
            }


            if (!File.Exists(filelocation))
            {
                using (StreamWriter sw = File.CreateText(filelocation))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("------------------------------");
                    sw.WriteLine(error);


                    sw.Close();

                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filelocation))
                {
                    sw.WriteLine(DateTime.Now.ToString());
                    sw.WriteLine("------------------------------");
                    sw.WriteLine(error);

                    sw.Close();
                }
            }
        }

        public static void WriteLog(string eventId, string filelocation)
        {
            filelocation = string.IsNullOrEmpty(filelocation) ? AppDomain.CurrentDomain.BaseDirectory + "\\ProcessLog\\" + DateTime.Now.ToString("ddMMyyy") + "_ProcessLog.txt": filelocation;
            if (!Directory.Exists(Path.GetDirectoryName(filelocation)))
            {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "\\ProcessLog\\");
            }
            if (!File.Exists(filelocation))
            {
                using (StreamWriter sw = File.CreateText(filelocation))
                {
                    sw.WriteLine($"{eventId}|{DateTime.Now}");
                    sw.Close();

                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filelocation))
                {
                    sw.WriteLine($"{eventId}|{DateTime.Now.ToString()}");
                    sw.Close();

                    sw.Close();
                }
            }
        }
    }
}