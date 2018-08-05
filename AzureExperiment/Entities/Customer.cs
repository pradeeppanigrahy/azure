using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureExperiment.Entities
{
    public class Customer :TableEntity
    {
        public Customer()
        {

        }
        public Customer(string firstname,string lastname)
        {
            base.PartitionKey = firstname;
            base.RowKey = lastname;
        }
        public string Name { get; set; }
        public string Address { get; set; }
        public int id { get; set; }
    }
}
