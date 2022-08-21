using System;
using System.Diagnostics;
using System.IO;

namespace ServiceDemo1.Utilities
{
    public class Logger
    {
        private static string GetDateFormat() => $"{DateTime.Now:dd/MM/yyyy hh:mm:ss tt}";


        public void Log_Exception(Exception exception)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + "//" +
                           Process.GetCurrentProcess().ProcessName + "_ErrorLog.txt";
            using (var writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("====================================================================================================================");
                writer.WriteLine($"Date : {GetDateFormat()}");
                writer.WriteLine();
                writer.WriteLine($"Message : {exception.Message}");
                writer.WriteLine($"StackTrace : {exception.StackTrace}");
                writer.WriteLine($"Source : {exception.Source}");
                writer.WriteLine($"InnerException : {exception.InnerException}");
                writer.WriteLine();
            }
        }


        public void WriteError(string message)
        {
            var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            var dateFormat = GetDateFormat();
            sw.WriteLine(dateFormat + "   Error:  " + message);
            sw.Flush();
            sw.Close();
        }


        public void WriteInfo(string message)
        {
            var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            var dateFormat = GetDateFormat();
            sw.WriteLine(dateFormat + "   Info:   " + message);
            sw.Flush();
            sw.Close();
        }


        public void Division()
        {
            var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            sw.WriteLine("====================================================================================================================");
            sw.Flush();
            sw.Close();
        }


        public void WriteFix(string message)
        {
            var sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            var dateFormat = GetDateFormat();
            sw.WriteLine(dateFormat + "   Fix:    " + message);
            sw.Flush();
            sw.Close();
        }
    }
}
