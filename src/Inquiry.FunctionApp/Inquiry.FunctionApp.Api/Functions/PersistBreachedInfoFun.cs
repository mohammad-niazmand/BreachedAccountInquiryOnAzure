using Inquiry.Func.Abstractions;
using Inquiry.Func.Models;
using Inquiry.Func.Validators;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Inquiry.Func.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public class PersistBreachedInfoFun
    {
        private readonly IBreachInquiryService _breachServices;

        public PersistBreachedInfoFun(IBreachInquiryService breachServices)
        {
            _breachServices = breachServices ?? throw new ArgumentNullException(nameof(breachServices));
        }

        [FunctionName("ProcessRequests")]
        public async Task Run(
            [QueueTrigger("azuretestappqueue")] BreachInquiryRequest myQueueItem,
            [CosmosDB(
            databaseName: "azure-test-DB",
            collectionName: "BreachedAccounts",
            ConnectionStringSetting = "AzureCosmosDBConnectionString")] IAsyncCollector<BreachedInfo> breachedInfoCosmosDB,
            ILogger _log
           )
        {
            _log.LogInformation($"Processing breached account requests for account: {myQueueItem.Account} is starting.");

            var (isValid, message) = BreachInquiryValidator.Validate(myQueueItem);
            if (isValid == false) throw new ArgumentException(message);

            var (isBreached,breachedInfo) = await _breachServices.ValidateAccountAsync(myQueueItem.Account);
            if (isBreached)
            {
                await breachedInfoCosmosDB.AddAsync(breachedInfo);
                _log.LogInformation($"Breached account info for account:{myQueueItem.Account} persisted.");
            }


        }
    }
}
