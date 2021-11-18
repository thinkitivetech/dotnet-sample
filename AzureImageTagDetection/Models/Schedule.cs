using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class Schedule
    {
        public Schedule()
        {
            Opening = new HashSet<Opening>();
        }

        public int ScheduleId { get; set; }
        public int LocationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual Location Location { get; set; }
        public virtual ICollection<Opening> Opening { get; set; }
    }
}
