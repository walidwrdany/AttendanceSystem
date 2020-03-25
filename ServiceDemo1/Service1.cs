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
using System.Threading;

namespace ServiceDemo1
{
    public partial class Service1 : ServiceBase
    {
        private Timer Schedular;
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

            int rId = 8;
            string name = Enum.GetName(typeof(SessionChangeReason), rId);
            DateTime now = DateTime.Now;

            AddNewActionsLog(rId, name, now);
        }

        protected override void OnStart(string[] args)
        {
            SaveNewEvent(ServiceEvents.ServiceStart);
            ScheduleService();
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

        private void SchedularCallback(object state)
        {
            ScheduleService();
        }

        private void ScheduleService()
        {
            Schedular = new Timer(new TimerCallback(SchedularCallback));
            TimeSpan timeSpan = new TimeSpan();

            DateTime now = DateTime.Now;
            DateTime scheduledTime = DateTime.Parse("09:30");
            DateTime SecondScheduledTime = DateTime.Parse("11:59");
            DateTime ThirdScheduledTime = DateTime.Parse("23:59");


            List<DateTime> dates = new List<DateTime>()
            {
                SecondScheduledTime,
                scheduledTime,
                ThirdScheduledTime
            }.OrderBy(x => x.TimeOfDay).ToList();


            EmailUtility.SendEmail("from_mail", "from_mail_name", "to_mail", "cc", "bcc", "subject", "body");
            //string _format = "HH";

            //if (now.ToString(_format) == ThirdScheduledTime.ToString(_format))
            //    SendEmailSummary(reports.Where(a => a.ReportMode == "Daily"));

            //else if (now.ToString(_format) == SecondScheduledTime.ToString(_format))
            //    SendEmailSummary(reports.Where(a => a.ReportMode == "Weekly" && a.DayToSend == now.DayOfWeek.ToString()));

            //else if (now.ToString(_format) == scheduledTime.ToString(_format))
            //    SendEmailSummary(reports.Where(a => a.ReportMode == "Monthly" && a.DayToSend == now.Day.ToString()));




            var nextDateToRun = dates.Where(a => a >= now).Count() > 0 ? dates.Where(a => a >= now).FirstOrDefault() : dates.FirstOrDefault().AddDays(1);
            timeSpan = nextDateToRun.Subtract(now);

            _logger.WriteInfo($"Simple Service Scheduled to run after: {timeSpan.Days} Day(s) {timeSpan.Hours} Hour(s) {timeSpan.Minutes} Minute(s) {timeSpan.Seconds} Second(s)");

            int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);
            Schedular.Change(dueTime, Timeout.Infinite);
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            try
            {
                int rId = (int)changeDescription.Reason;
                string name = Enum.GetName(typeof(SessionChangeReason), rId);
                DateTime now = DateTime.Now;
                string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                string computerName = Environment.MachineName;

                _logger.WriteInfo(name + " / " + userName);
                AddNewActionsLog(rId, name, now);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.ToString());
            }
        }

        private void AddNewActionsLog(int rId, string name, DateTime now)
        {
            // To Get First SessionUnlock in Day
            bool isFirst = !_db.ActionsLogs.Any(a => DbFunctions.TruncateTime(a.ActionDate) == DbFunctions.TruncateTime(now));

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

                var yesterday = _db.ActionsLogs.GroupBy(g => DbFunctions.TruncateTime(g.ActionDate)).ToList().LastOrDefault();

                var workDay = _db.WorkDays.FirstOrDefault(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(yesterday.Key) && a.IsActive);
                if (workDay != null)
                {
                    var actionsLog = yesterday.LastOrDefault().ActionDate;
                    var _EndAt = new TimeSpan(actionsLog.Hour, actionsLog.Minute, actionsLog.Second);

                    workDay.EndAt = _EndAt;
                    workDay.TotalHour = _EndAt - workDay.StartAt;
                    _db.SaveChanges();
                }

            }
        }


        #region NetWork

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

        #endregion

    }
}
