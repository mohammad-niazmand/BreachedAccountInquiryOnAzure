using Inquiry.Func.Abstractions;
using Inquiry.Func.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Inquiry.Func.Services
{
    public class BreachInquiryService : IBreachInquiryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfigurationRoot _configuration;

        public BreachInquiryService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }

        public async Task<(bool isBreached, BreachedInfo breachedInfo)> ValidateAccountAsync(string accountName)
        {
            var httpRequestMessage = CreateRequestMessage(accountName);
            var responseMessage = await _httpClient.SendAsync(httpRequestMessage);
            var breachedInfo = await responseMessage.Content.ReadAsStringAsync();
            var jsonResult = JsonSerializer.Deserialize<BreachedInfo>(breachedInfo);

            if (jsonResult?.Name.Trim().Length > 0)
                return (true, jsonResult);

            return (false, null);
        }

        private HttpRequestMessage CreateRequestMessage(string account)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_configuration.GetValue<string>("BreachServiceBaseAddress")}/api/v3/breachedaccount/{account}")
            };

            httpRequestMessage.Headers.Add("hibp-api-key", _configuration["hibp-api-key"]);
            return httpRequestMessage;
        }
    }
}
