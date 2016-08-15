using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace OneAppAwayAnalytics
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }

        
        static void Run()
        {
            CloudStorageAccount account;
            CloudStorageAccount.TryParse("DefaultEndpointsProtocol=https;AccountName=one1app1away1analytics;AccountKey=Z7CENXZLbLlc6M7bhk/Q3lkChQ647SKsPKj4rkH+iPeaFhQGaxywhWYxVNhq7rkA1t37s96zAxuReGg0jf2cvw==", out account);
            var tableClient = account.CreateCloudTableClient();
            var testTable = tableClient.GetTableReference("Test");
            testTable.CreateIfNotExists();
            Test tst = new Test() { Num1 = 0, Num2 = 0 };
            TableOperation insert = TableOperation.Insert(tst);
            testTable.Execute(insert);
        }
    }

    class Test : TableEntity
    {
        public Test()
        {
            this.PartitionKey = "0";
            this.RowKey = DateTime.Now.ToString();
        }

        public int Num1 { get; set; }
        public int Num2 { get; set; }
    }
}
