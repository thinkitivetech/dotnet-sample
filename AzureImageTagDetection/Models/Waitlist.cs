using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class Waitlist
    {
        public int WaitlistId { get; set; }
        public int OpeningId { get; set; }
        public int SpareId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Comment { get; set; }

        public virtual Opening Opening { get; set; }
        public virtual Spare Spare { get; set; }
    }
}
