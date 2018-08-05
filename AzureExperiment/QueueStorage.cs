using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureExperiment
{
    public class QueueStorage
    {
        IConfiguration config;
        public QueueStorage(IConfiguration configuration)
        {
            config = configuration;
        }

        public async Task<CloudQueue> GetQueueReference()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.GetConnectionString("70532s_AzureStorageConnectionString"));
            var queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("tasks");
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        public async Task AddMessage(CloudQueue queue, string message)
        {
            await queue.AddMessageAsync(new CloudQueueMessage(message));

        }
        public async Task<CloudQueueMessage> GetMessage(CloudQueue queue)
        {
            return await queue.GetMessageAsync();
            
        }

        public async Task<CloudQueueMessage> PeekMessage(CloudQueue queue)
        {
            return await queue.PeekMessageAsync();
            
        }

        public async Task DeleteMessage(CloudQueue queue,CloudQueueMessage message)
        {
            await queue.DeleteMessageAsync(message);
        }
        public async Task ProcessMessagesAndDelete(CloudQueue queue)
        {
            foreach (CloudQueueMessage message in queue.GetMessagesAsync(20, TimeSpan.FromMinutes(5), null, null).Result)
            {
                // Process all messages in less than 5 minutes, deleting each message after processing.
                await queue.DeleteMessageAsync(message);
            }
            return;
        }
    }
}
