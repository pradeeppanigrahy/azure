using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureExperiment
{
    public class BlobStorage
    {
        IConfiguration config;
        public BlobStorage(IConfiguration configuration)
        {
            this.config = configuration;
        }

        public async Task UploadBlob()
        {
            var connString = config.GetConnectionString("70532s_AzureStorageConnectionString");
            var containerRef = await GetContainerReference(connString);
            var blobRef = containerRef.GetBlockBlobReference("firstfile");
            using (var fileStream = System.IO.File.OpenRead(@"c:\app\data\docker.txt"))
            {
                await blobRef.UploadFromStreamAsync(fileStream);
            }
        }

        public async Task DownloadBlob()
        {
            var connString = config.GetConnectionString("70532s_AzureStorageConnectionString");
            var containerRef = await GetContainerReference(connString);
            var blobRef = containerRef.GetBlockBlobReference("firstfile");
            string sasToekn=GetSharedAccessSignature(blobRef);
            using (var fileStream = System.IO.File.OpenWrite(@"c:\app\data\dockerfromcloud.txt"))
            {
                await blobRef.DownloadToStreamAsync(fileStream);
            }
        }

        private string GetSharedAccessSignature(CloudBlob blobRef)
        {
            SharedAccessBlobPolicy policy = new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List,
                SharedAccessStartTime = DateTime.Now,
                SharedAccessExpiryTime = DateTime.Now.AddMinutes(30)
            };
            return blobRef.GetSharedAccessSignature(policy);
        }

        public async Task DeleteBlob()
        {
            var connString = config.GetConnectionString("70532s_AzureStorageConnectionString");
            var containerRef = await GetContainerReference(connString);
            var blobRef = containerRef.GetBlockBlobReference("firstfile");
            await blobRef.DeleteIfExistsAsync();
        }

        public async Task<List<string>> ListBlob()
        {
            var connString = config.GetConnectionString("70532s_AzureStorageConnectionString");
            var containerRef = await GetContainerReference(connString);
            
            var blobRef = containerRef.GetBlockBlobReference("firstfile");
            IEnumerable<ListBlockItem> result = await blobRef.DownloadBlockListAsync();
            List<string> blobitems = new List<string>();
            foreach(var item in result)
            {
                blobitems.Add( item.Name);
            }
            return blobitems;
        }



        private async Task<CloudBlobContainer> GetContainerReference(string connString)
        {
            var storageAccount = CloudStorageAccount.Parse(connString);
            
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerRef = blobClient.GetContainerReference("files");
            //await containerRef.DeleteIfExistsAsync().ContinueWith((x) =>
            //{
            //    if (x.IsCompletedSuccessfully)
            //        containerRef.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext() { });
            //}
            //);
            await containerRef.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, new BlobRequestOptions(), new OperationContext() { });
            return containerRef;
        }
    }
}
