using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceDemo1.DataModel
{
    public class WorkDays
    {
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int FK_StatusId { get; set; }

        public TimeSpan? TotalHour { get; set; }

        public TimeSpan? StartAt { get; set; }

        public TimeSpan? EndAt { get; set; }

        public bool IsActive { get; set; }
    }
}
