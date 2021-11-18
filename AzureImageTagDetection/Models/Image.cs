using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class Image
    {
        public int ImageId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? LastModified { get; set; }
        public string ExternalKey { get; set; }
        public string Tags { get; set; }
    }
}
