using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class SpareAttribute
    {
        public int SpareAttributeId { get; set; }
        public int SpareId { get; set; }
        public string AttributeKey { get; set; }
        public int? AttributeValue { get; set; }
        public string AttributeSet { get; set; }

        public virtual Spare Spare { get; set; }
    }
}
