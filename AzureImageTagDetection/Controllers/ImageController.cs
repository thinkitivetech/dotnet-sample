using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Microsoft.AspNetCore.Http;
using AzureImageTagDetection.BL;
using AzureImageTagDetection.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureImageTagDetection.Controllers
{
    public class ImageController : Controller
    {
        readonly ImageTagBLManager service = new ImageTagBLManager();
        private readonly Mykeys _appSettings;

        public ImageController(IOptions<Mykeys> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public ActionResult UploadImage()
        {
            return View();
        }
        /// <summary>
        /// Upload Image to Azure Server
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile files)
        {
            ImageViewModel entity_data = new ImageViewModel();
            try
            {
                string sub_key = _appSettings.Subscriptionkey;
                string api_url = _appSettings.VisionAPIURL;
                entity_data = await service.UploadImageAsync(files, sub_key, api_url);
            }
            catch (Exception ex)
            {
                ExceptionLog.ErrorLogging(ex);
            }
            return View("UploadImage", entity_data);
        }
    }
}
