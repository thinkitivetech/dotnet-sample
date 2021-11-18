using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureImageTagDetection.Models;

namespace AzureImageTagDetection.Interface
{
    interface IImageRepository
    {
        int addImageTags(string name,string path,string ImageTags, string RequestId);
    }
}
