using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DockerExperiment.Pages
{
    public class IndexModel : PageModel
    {
        IConfiguration config;
        public IndexModel(IConfiguration configuration)
        {
            this.config = configuration;
        }
        public async void OnGet()
        {
            var connString = config.GetConnectionString("70532s_AzureStorageConnectionString");
            var containerRef= await GetContainerReference(connString);
            var blobRef = containerRef.GetBlockBlobReference("firstfile");
            using (var fileStream = System.IO.File.OpenRead(@"c:\app\data\docker.txt"))
            {
                await blobRef.UploadFromStreamAsync(fileStream);
            }
           
        }

        private async Task<CloudBlobContainer> GetContainerReference(string connString)
        {
            var storageAccount = CloudStorageAccount.Parse(connString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerRef = blobClient.GetContainerReference("files");
            await containerRef.DeleteIfExistsAsync().ContinueWith((x) =>
            {
                if (x.IsCompletedSuccessfully)
                     containerRef.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext() { });
            }
            );
            return containerRef;
        }
    }
}
