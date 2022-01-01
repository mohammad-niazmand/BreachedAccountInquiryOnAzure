using Inquiry.Func.Extensions;
using Inquiry.Func.Models;
using Inquiry.Func.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Inquiry.Func.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public class BreachedInquiryFun
    {
        [FunctionName("Breaches")]
        public async Task<ActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("azuretestappqueue")] IAsyncCollector<BreachInquiryRequest> verifyBreachRequestQueue,
            CancellationToken cancellationToken,
            ILogger log)
        {
            log.LogInformation("Start processing a verify breach request.");

            var verifyBreachRequest = await req.ReadAsJsonAsync<BreachInquiryRequest>(cancellationToken);

            var (isValid, message) = BreachInquiryValidator.Validate(verifyBreachRequest);
            if (isValid == false) return new BadRequestObjectResult(message);

            await verifyBreachRequestQueue.AddAsync(verifyBreachRequest);

            log.LogInformation("Verify breach request added to the queue.");

            return new OkObjectResult($"Your request for account: {verifyBreachRequest.Account} registered successfully.");
        }

    }
}
