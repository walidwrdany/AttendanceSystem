using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDemo1
{
    public class Logger
    {
        private string GetDateFormat() => String.Format("{0:dd/MM/yyyy hh:mm:ss tt}", DateTime.Now);
        public void WriteError(string Message)
        {
            StreamWriter SW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            string dateFormat = GetDateFormat();
            SW.WriteLine(dateFormat + "   Error:  " + Message);
            SW.Flush();
            SW.Close();
        }
        public void WriteInfo(string Message)
        {
            StreamWriter SW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            string dateFormat = GetDateFormat();
            SW.WriteLine(dateFormat + "   Info:   " + Message);
            SW.Flush();
            SW.Close();
        }
        public void Division()
        {
            StreamWriter SW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            SW.WriteLine("====================================================================================================================");
            SW.Flush();
            SW.Close();
        }
        public void WriteFix(string Message)
        {
            StreamWriter SW = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "//" + Process.GetCurrentProcess().ProcessName + "_LOG.txt", true);
            string dateFormat = GetDateFormat();
            SW.WriteLine(dateFormat + "   Fix:    " + Message);
            SW.Flush();
            SW.Close();
        }
    }
}
