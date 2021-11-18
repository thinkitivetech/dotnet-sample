using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;
using AzureImageTagDetection.DAL;
using System.Linq;
using AzureImageTagDetection.Models;

namespace AzureImageTagDetection.BL
{
    public class ImageTagBLManager
    {
        public readonly ImageRepository ImgRepo = new ImageRepository();        
        /// <summary>
        /// Upload Image to Server
        /// </summary>
        /// <param name="imageToUpload"></param>
        /// <param name="SubKey"></param>
        /// <returns></returns>
        public async Task<ImageViewModel> UploadImageAsync(IFormFile imageToUpload, string SubKey,string api_url)
        {
            ImageViewModel entity_data = new ImageViewModel();
            if (imageToUpload == null || imageToUpload.Length == 0)
            {
                return null;
            }
            try
            {
                //Get Azure Container Details
                CloudStorageAccount cloudStorageAccount = ConnectionString.GetConnectionString();
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("sampleimage");

                #region Upload Image to Azure Blob
                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                }
                string imageName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(imageToUpload.FileName);

                CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(imageName);
                cloudBlockBlob.Properties.ContentType = imageToUpload.ContentType;

                await cloudBlockBlob.UploadFromStreamAsync(imageToUpload.OpenReadStream());

                string imageFullPath = cloudBlockBlob.Uri.ToString();
                #endregion
                #region Send Image Data for Analysis
                var ResponseData = await MakeAnalysisRequest(imageName, imageFullPath, SubKey, api_url);
                entity_data.ResponseData = ResponseData.Tags;
                entity_data.Description = ResponseData.description;
                entity_data.UrlPath = imageFullPath;
                entity_data.Objects = ResponseData.objects;
                entity_data.ImageId = ResponseData.ImageId;
                #endregion
            }
            catch (Exception ex)
            {
                ExceptionLog.ErrorLogging(ex);
            }
            return entity_data;
        }

        public async Task<ImageInfoViewModel> MakeAnalysisRequest(string name, string path, string sub_key,string api_url)
        {
            JObject objurl = new JObject
            {
                { "url", path }
            };

            var errors = new List<string>();
            ImageInfoViewModel responeData = new ImageInfoViewModel();
            var result = "";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", sub_key);
                    var uri = api_url;
                    HttpResponseMessage response;
                    byte[] byteData = Encoding.UTF8.GetBytes(objurl.ToString());

                    using (var content = new ByteArrayContent(byteData))
                    {
                        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        response = await client.PostAsync(uri, content);
                        result = await response.Content.ReadAsStringAsync();
                        if (response.IsSuccessStatusCode)
                        {
                            responeData = JsonConvert.DeserializeObject<ImageInfoViewModel>(result, new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Include,
                                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
                                {
                                    errors.Add(earg.ErrorContext.Member.ToString());
                                    earg.ErrorContext.Handled = true;
                                }
                            });
                        }
                        string imageTags = string.Empty;
                        try
                        {
                            var Json = JObject.Parse(result);

                            JArray jsonArray = JArray.Parse(Json["objects"].ToString());
                            int i = 0;
                            foreach (var jsonKey in jsonArray)
                            {
                                var objectName = jsonKey["object"].ToString();
                                responeData.objects[0].objectName = objectName;
                                i++;
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionLog.ErrorLogging(ex);
                        }
                        if (responeData != null)
                        {
                            imageTags = string.Join(',', responeData.Tags.Select(x => x.name).ToList());
                        }
                        responeData.ImageId = ImgRepo.addImageTags(name, path, imageTags, responeData.RequestId);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLog.ErrorLogging(ex);
            }
            return responeData;
        }
    }
}
