using ServiceDemo1.DataModel;
using System;
using System.Data.Entity;
using System.Linq;
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
            OnStart(null);
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

                _logger.WriteInfo(name);

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

                    var yesterdayActions = _db.ActionsLogs.LastOrDefault(a => DbFunctions.TruncateTime(a.ActionDate) == DbFunctions.TruncateTime(now.AddDays(-1)));
                    var WorkYesterday = _db.WorkDays.FirstOrDefault(a => DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(now.AddDays(-1)) && a.IsActive);
                    var _EndAt = new TimeSpan(yesterdayActions.ActionDate.Hour, yesterdayActions.ActionDate.Minute, yesterdayActions.ActionDate.Second);
                    WorkYesterday.EndAt = _EndAt;
                    WorkYesterday.TotalHour = _EndAt - WorkYesterday.StartAt;
                    _db.SaveChanges();
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
    }
}
