using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceDemo1.DataModel
{
    public class ActionsLog
    {
        public int Id { get; set; }

        public int ActionId { get; set; }

        [StringLength(100)]
        public string ActionName { get; set; }

        public DateTime ActionDate { get; set; }

        public bool IsFirst { get; set; }
    }
}
