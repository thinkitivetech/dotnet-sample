using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace AzureImageTagDetection.Models
{
    public class ConnectionString
    {      
        public static CloudStorageAccount GetConnectionString()
        {            
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName=testinterview1;AccountKey=PeQSBkwPGWDi4N6pv9OX/jG3dSuJHz9gChmraCdnNZxtEg4lhr050krLw1dT3CYxSIgP6z7u7uFX4fSOMEQBiA==;EndpointSuffix=core.windows.net");
            return CloudStorageAccount.Parse(connectionString);
        }

        
    }
}
