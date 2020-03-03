using ServiceDemo1.DataModel;
using ServiceDemo1.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;

namespace ServiceDemo1
{
    public partial class Service1 : ServiceBase
    {
        private Logger _logger;
        private readonly AttendanceSystem _db;

        public Service1()
        {
            InitializeComponent();

            this.CanHandleSessionChangeEvent = true;

            //Some other things you can enable here
            this.AutoLog = true;
            this.CanHandlePowerEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanStop = true;

            _logger = new Logger();
            _db = new AttendanceSystem();
            _db.Database.CreateIfNotExists();
        }

        internal void OnDebug()
        {
            //OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            SaveNewEvent(ServiceEvents.ServiceStart);
        }

        protected override void OnStop()
        {
            SaveNewEvent(ServiceEvents.ServiceStop);
        }

        protected override void OnPause()
        {
            SaveNewEvent(ServiceEvents.ServicePause);
        }

        protected override void OnContinue()
        {
            SaveNewEvent(ServiceEvents.ServiceContinue);
        }

        protected override void OnShutdown()
        {
            SaveNewEvent(ServiceEvents.ServiceShutdown);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            try
            {
                int rId = (int)changeDescription.Reason;
                string name = Enum.GetName(typeof(SessionChangeReason), rId);
                DateTime now = DateTime.Now;
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string computerName = System.Environment.MachineName;


                _logger.WriteInfo(name + " / " + userName + " / " + computerName);

                // To Get First SessionUnlock in Day
                bool isFirst = rId == 8 ? !_db.ActionsLogs.Any(a => DbFunctions.TruncateTime(a.ActionDate) == DbFunctions.TruncateTime(now) && a.ActionId == rId) : false;

                _db.ActionsLogs.Add(
                    new ActionsLog
                    {
                        ActionId = rId,
                        ActionName = name,
                        ActionDate = now,
                        IsFirst = isFirst
                    });
                _db.SaveChanges();

                if (isFirst)
                {
                    _db.WorkDays.Add(
                        new WorkDays
                        {
                            Date = now,
                            FK_StatusId = 1,
                            StartAt = new TimeSpan(now.Hour, now.Minute, now.Second),
                            IsActive = true
                        });
                    _db.SaveChanges();
                    UpdateYesterday();
                }
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.ToString());
            }
        }

        private void SaveNewEvent(ServiceEvents serviceEvents)
        {
            try
            {
                string eventName = Enum.GetName(typeof(ServiceEvents), serviceEvents);

                _logger.WriteInfo(eventName);

                _db.ActionsLogs.Add(
                    new ActionsLog
                    {
                        ActionId = (int)serviceEvents,
                        ActionName = eventName,
                        ActionDate = DateTime.Now,
                    });
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.ToString());
            }
        }

        private int UpdateYesterday()
        {
            DateTime now = DateTime.Now;
            DateTime yesterDate = now.AddDays(-1);

            List<ActionsLog> actionsLogs = _db.ActionsLogs.Where(a => DbFunctions.TruncateTime(a.ActionDate).Value == DbFunctions.TruncateTime(yesterDate).Value).ToList();
            WorkDays workDays = _db.WorkDays.FirstOrDefault(a => DbFunctions.TruncateTime(a.Date).Value == DbFunctions.TruncateTime(yesterDate).Value && a.IsActive);

            if (workDays != null)
            {

                var actionsLog = actionsLogs.Where(a => a.ActionId == (int)SessionChangeReason.SessionLock).LastOrDefault();
                var _EndAt = new TimeSpan(actionsLog.ActionDate.Hour, actionsLog.ActionDate.Minute, actionsLog.ActionDate.Second);

                workDays.EndAt = _EndAt;
                workDays.TotalHour = _EndAt - workDays.StartAt;
                return _db.SaveChanges();
            }

            return 0;
        }

        private bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        private string GetLocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToString();
        }

        private string GetPublicIPAddress()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://ipinfo.io/ip");

            request.UserAgent = "curl"; // this will tell the server to return the information as if the request was made by the linux "curl" command

            string publicIPAddress;

            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    publicIPAddress = reader.ReadToEnd();
                }
            }

            return publicIPAddress.Replace("\n", "");
        }


    }
}
