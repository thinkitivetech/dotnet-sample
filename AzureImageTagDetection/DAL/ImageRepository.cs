using AzureImageTagDetection.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureImageTagDetection.Models;

namespace AzureImageTagDetection.DAL
{
    public class ImageRepository : IImageRepository
    {


        public int addImageTags(string name, string path, string ImageTags, string RequestId)
        {
            try
            {
                using (DBContext dbContext = new DBContext())
                {
                    Image imageObj = new Image();
                    imageObj.Name = name;
                    imageObj.Path = path;
                    imageObj.Tags = ImageTags;
                    imageObj.CreateDate = DateTime.Now;
                    imageObj.LastModified = DateTime.Now;
                    imageObj.ExternalKey = RequestId;
                    dbContext.Image.Add(imageObj);
                    int rowcreated = dbContext.SaveChanges();
                    if (rowcreated > 0)
                        return imageObj.ImageId;
                    else
                        return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}
