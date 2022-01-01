using Inquiry.Func;
using Inquiry.Func.Abstractions;
using Inquiry.Func.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Inquiry.Func
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddHttpClient<IBreachInquiryService, BreachInquiryService>();
            builder.Services.Configure<HttpClientFactoryOptions>(options => options.SuppressHandlerScope = true);
        }
    }
}
