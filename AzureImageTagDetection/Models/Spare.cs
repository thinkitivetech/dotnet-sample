using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class Spare
    {
        public Spare()
        {
            Opening = new HashSet<Opening>();
            SpareAttribute = new HashSet<SpareAttribute>();
            SparePhoto = new HashSet<SparePhoto>();
            Waitlist = new HashSet<Waitlist>();
        }

        public int SpareId { get; set; }
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string TimeZone { get; set; }
        public string Gender { get; set; }
        public DateTime? Birthdate { get; set; }
        public DateTime CreateDate { get; set; }
        public string Ip { get; set; }
        public int? Priority { get; set; }

        public virtual ICollection<Opening> Opening { get; set; }
        public virtual ICollection<SpareAttribute> SpareAttribute { get; set; }
        public virtual ICollection<SparePhoto> SparePhoto { get; set; }
        public virtual ICollection<Waitlist> Waitlist { get; set; }
    }
}
