using ServiceDemo1.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Threading;
using ServiceDemo1.Utilities;

namespace ServiceDemo1
{
    public partial class Service1 : ServiceBase
    {

        private Timer _timer;
        private readonly Logger _logger;
        private readonly AttendanceSystem _db;


        public Service1()
        {
            InitializeComponent();

            CanHandleSessionChangeEvent = true;

            //Some other things you can enable here
            AutoLog = true;
            CanHandlePowerEvent = true;
            CanPauseAndContinue = true;
            CanShutdown = true;
            CanStop = true;

            _logger = new Logger();
            _db = new AttendanceSystem();

        }

        internal void OnDebug()
        {
            //OnStart(null);

            const int rId = 4;
            var name = Enum.GetName(typeof(SessionChangeReason), rId);
            AddNewActionsLog(rId, name);
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
                var eventId = (int)serviceEvents;
                var eventName = Enum.GetName(typeof(ServiceEvents), serviceEvents);
                _logger.WriteInfo(eventName);
                AddNewActionsLog(eventId, eventName);
            }
            catch (Exception ex)
            {
                _logger.Log_Exception(ex);
            }
        }


        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            try
            {
                var rId = (int)changeDescription.Reason;
                var name = Enum.GetName(typeof(SessionChangeReason), rId);
                var userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                var computerName = Environment.MachineName;
                _logger.WriteInfo(name + " / " + userName);
                AddNewActionsLog(rId, name);
            }
            catch (Exception ex)
            {
                _logger.Log_Exception(ex);
            }
        }

        private void AddNewActionsLog(int rId, string name)
        {
            var now = DateTime.Now;
            // To Get First SessionUnlock in Day
            var isFirst = !_db.ActionsLogs.Any(a => DbFunctions.TruncateTime(a.ActionDate) == DbFunctions.TruncateTime(now));

            _db.ActionsLogs.Add(
                new ActionsLog
                {
                    ActionId = rId,
                    ActionName = name,
                    ActionDate = now,
                    IsFirst = isFirst
                });
            _db.SaveChanges();

            if (!isFirst) return;
            _db.WorkDays.Add(
                new WorkDays
                {
                    Date = now,
                    FK_StatusId = 1,
                    StartAt = new TimeSpan(now.Hour, now.Minute, now.Second),
                    IsActive = true
                });
            _db.SaveChanges();
        }

        private void Data()
        {
            if (true)
            {
                var now = DateTime.Now;
                var yesterday = _db.ActionsLogs.GroupBy(g => DbFunctions.TruncateTime(g.ActionDate)).ToList().LastOrDefault();
                var workDay = _db.WorkDays.FirstOrDefault(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(yesterday.Key) && a.IsActive);
                if (workDay == null) return;
                var actionsLog = yesterday.LastOrDefault().ActionDate;
                var endAt = new TimeSpan(actionsLog.Hour, actionsLog.Minute, actionsLog.Second);

                workDay.EndAt = endAt;
                workDay.TotalHour = endAt - workDay.StartAt;
                _db.SaveChanges();

            }
        }


        private void Callback(object state)
        {
            ScheduleService();
        }

        private void ScheduleService()
        {
            _timer = new Timer(Callback);

            var now = DateTime.Now;
            var dates = new List<DateTime>
            {
                DateTime.Parse("09:30"),
                DateTime.Parse("11:59"),
                DateTime.Parse("23:59")
            }
                .OrderBy(x => x.TimeOfDay)
                .ToList();

            var nextDateToRun = dates.Any(a => a >= now) ? dates.FirstOrDefault(a => a >= now) : dates.FirstOrDefault().AddDays(1);
            var timeSpan = nextDateToRun.Subtract(now);

            _logger.WriteInfo(
                $"Simple Service Scheduled to run after: {timeSpan.Days} Day(s) {timeSpan.Hours} Hour(s) {timeSpan.Minutes} Minute(s) {timeSpan.Seconds} Second(s)");

            var dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);
            _timer.Change(dueTime, Timeout.Infinite);
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

        private string GetLocalIpAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                ?.ToString();
        }



        #endregion

    }
}
