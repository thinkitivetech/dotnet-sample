using System;
using System.Collections.Generic;

namespace AzureImageTagDetection.Models
{
    public partial class StaffPhoto
    {
        public int StaffPhotoId { get; set; }
        public int StaffId { get; set; }
        public int PhotoId { get; set; }
        public byte? PermissionLevel { get; set; }

        public virtual Photo Photo { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
