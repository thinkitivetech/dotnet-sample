using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class Location
    {
        public Location()
        {
            Schedule = new HashSet<Schedule>();
        }

        public int LocationId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public string PostalZip { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }

        public virtual ICollection<Schedule> Schedule { get; set; }
    }
}
