using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class Opening
    {
        public Opening()
        {
            Waitlist = new HashSet<Waitlist>();
        }

        public int OpeningId { get; set; }
        public int? ScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int StaffId { get; set; }
        public int? SpareId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? FillDate { get; set; }
        public string Note { get; set; }

        public virtual Schedule Schedule { get; set; }
        public virtual Spare Spare { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual ICollection<Waitlist> Waitlist { get; set; }
    }
}
