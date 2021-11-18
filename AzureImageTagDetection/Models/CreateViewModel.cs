using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureImageTagDetection.Interface;

namespace AzureImageTagDetection.Models
{
    public class CreateViewModel 
    { 
        public IFormFile photo { get; set; }
    }
}
