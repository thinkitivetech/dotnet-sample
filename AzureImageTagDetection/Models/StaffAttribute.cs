using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class StaffAttribute
    {
        public int StaffAttributeId { get; set; }
        public int StaffId { get; set; }
        public string AttributeKey { get; set; }
        public int? AttributeValue { get; set; }
        public string AttributeSet { get; set; }

        public virtual Staff Staff { get; set; }
    }
}
