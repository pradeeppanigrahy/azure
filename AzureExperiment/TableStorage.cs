using AzureExperiment.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureExperiment
{
    public class TableStorage
    {

        //Check out linq extensions 
        IConfiguration config;
        public TableStorage(IConfiguration configuration)
        {
            this.config = configuration;
        }

        public async Task<CloudTable> GetTableReference()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config.GetConnectionString("70532s_AzureStorageConnectionString"));
            var tableClient=storageAccount.CreateCloudTableClient();
            CloudTable table=tableClient.GetTableReference("customers");
            await table.CreateIfNotExistsAsync();
            return table;
        }
        public async Task<bool> CreateCustomer(Customer customer)
        {
            var table = await GetTableReference();
            await CreateCustomer(table, customer);
            return true;
        }

        public async Task<Customer> GetCustomer(string partitionKey,string rowKey)
        {
            var table = await GetTableReference();
            return await GetCustomer(table,partitionKey,rowKey );
            
        }

        public async Task<TableQuerySegment<Customer>> GetAllCustomer(string partitionKey, TableContinuationToken token)
        {
            var table = await GetTableReference();
            return await GetAllCustomer(table, partitionKey,token);

        }

        public async Task<bool> DeleteCustomer(Customer customer)
        {
            var table = await GetTableReference();
            await DeleteCustomer(table, customer);
            return true;
        }

        public async Task<IList<TableResult>> CreateCustomerBatch(List<Customer> customers)
        {
            var table = await GetTableReference();
            return await CreateCustomerBatch(table,customers);
        }

        #region private
        private async Task<TableResult> CreateCustomer(CloudTable table,Customer customer)
        {
            TableOperation operation = TableOperation.InsertOrReplace(customer);
            return await table.ExecuteAsync(operation);
        }

        private async Task<TableResult> DeleteCustomer(CloudTable table, Customer customer)
        {
            TableOperation operation = TableOperation.Replace(customer);
            return await table.ExecuteAsync(operation);
        }

        private async Task<Customer> GetCustomer(CloudTable table,string partitonkey,string rowkey)
        {
            TableOperation operation = TableOperation.Retrieve<Customer>(partitonkey,rowkey);
            TableResult result=await table.ExecuteAsync(operation);
            return (Customer)result.Result;
        }

        private async Task<TableQuerySegment<Customer>> GetAllCustomer(CloudTable table, string partitonkey, TableContinuationToken token)
        {
            TableQuery<Customer> tableQuery = new TableQuery<Customer>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                QueryComparisons.Equal, partitonkey));
            
            //TableContinuationToken token = null;
            return await table.ExecuteQuerySegmentedAsync(tableQuery,token);
            
        }

        private async Task<IList<TableResult>> CreateCustomerBatch(CloudTable table, List<Customer> customers)
        {
            TableBatchOperation tableOperations = new TableBatchOperation();
            customers.ForEach(cust => tableOperations.InsertOrReplace(cust));
            return await table.ExecuteBatchAsync(tableOperations);
        }

      
        #endregion
    }
}
