using Microsoft.WindowsAzure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureImageTagDetection.Models
{
    public class ImageViewModel
    {
        public List<Tags> ResponseData { get; set; }

        public Description Description { get; set; }
        public List<objectItem> Objects { get; set; }
        public string UrlPath { get; set; }
        public int ImageId { get; set; }
    }

    public class JSONImageData
    {
        public ImageViewModel ResponseData { get; set; }
    }

   
   
}
