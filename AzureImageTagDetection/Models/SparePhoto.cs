using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class SparePhoto
    {
        public int SparePhotoId { get; set; }
        public int SpareId { get; set; }
        public int PhotoId { get; set; }
        public byte? PermissionLevel { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual Spare Spare { get; set; }
    }
}
