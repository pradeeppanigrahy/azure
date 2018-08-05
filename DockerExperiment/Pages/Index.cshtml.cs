using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureExperiment;
using AzureExperiment.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

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
            //Blob storage operations
            //await BlobStorageOperations();

            //Table storage operations
            //await TableStorageOperations();

            //Queue storage operations
            await QueueStorageOperations();
        }

        private async Task QueueStorageOperations()
        {
            QueueStorage queueStorage = new QueueStorage(config);
            CloudQueue cloudQueue = await queueStorage.GetQueueReference();
            await queueStorage.AddMessage(cloudQueue, "resize image");
            var result = await queueStorage.PeekMessage(cloudQueue);
            var message = await queueStorage.GetMessage(cloudQueue);
            await queueStorage.DeleteMessage(cloudQueue, message);
            result = await queueStorage.GetMessage(cloudQueue);

            Enumerable.Range(1, 10).ToList().ForEach(async t => await queueStorage.AddMessage(cloudQueue, $"resize image {t}"));
            message = await queueStorage.GetMessage(cloudQueue);
            message = await queueStorage.GetMessage(cloudQueue);
            await queueStorage.ProcessMessagesAndDelete(cloudQueue);
            message = await queueStorage.GetMessage(cloudQueue);
        }

        private async Task BlobStorageOperations()
        {
            BlobStorage blobStorage = new BlobStorage(config);
            await blobStorage.UploadBlob();
            await blobStorage.DownloadBlob();
            var result = await blobStorage.ListBlob();
        }

        private async Task TableStorageOperations()
        {
            TableStorage tableStorage = new TableStorage(config);
            await tableStorage.CreateCustomer(new Customer("pradeep", "panigrahy") { Name = "pradeep", Address = "pune", id = 1 });
            var result = await tableStorage.GetCustomer("pradeep", "panigrahy");
            List<Customer> customers = new List<Customer>();
            Enumerable.Range(1, 10).ToList().ForEach(t => customers.Add(new Customer("firstpartition", $"row{t}")
            {
                Name = $"firstname{t}",
                Address = $"address{t}"
            }));
            var batchResult = await tableStorage.CreateCustomerBatch(customers);


            TableContinuationToken token = null;
            
            do
            {
                var getBatchResult = await tableStorage.GetAllCustomer("firstpartition",token);
                token = getBatchResult.ContinuationToken;
                foreach (Customer cust in getBatchResult.Results)
                {
                    customers.Clear();
                    customers.Add(cust);
                }
            } while (token != null);
            token = null;
            var getBatchResult1 = await tableStorage.GetAllCustomer("pradeep",token);
        }

    }
}
