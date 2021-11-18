using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class AttributeEnumeration
    {
        public string AttributeKey { get; set; }
        public int? AttributeValue { get; set; }
        public string AttributeSet { get; set; }
    }
}
