namespace ServiceDemo1.DataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class AttendanceSystem : DbContext
    {
        public AttendanceSystem()
            : base("name=AttendanceSystem")
        {
        }

        public virtual DbSet<ActionsLog> ActionsLogs { get; set; }
        public virtual DbSet<WorkDays> WorkDays { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
