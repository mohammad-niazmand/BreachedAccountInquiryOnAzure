using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WireMock.Admin.Requests;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Net.WebApplication.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.WebApplication
{
    public class WireMockService : IWireMockService
    {
        private WireMockServer _server;
        private readonly ILogger _logger;
        private readonly WireMockServerSettings _settings;

        private class Logger : IWireMockLogger
        {
            private readonly ILogger _logger;

            public Logger(ILogger logger)
            {
                _logger = logger;
            }

            public void Debug(string formatString, params object[] args)
            {
                _logger.LogDebug(formatString, args);
            }

            public void Info(string formatString, params object[] args)
            {
                _logger.LogInformation(formatString, args);
            }

            public void Warn(string formatString, params object[] args)
            {
                _logger.LogWarning(formatString, args);
            }

            public void Error(string formatString, params object[] args)
            {
                _logger.LogError(formatString, args);
            }

            public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminrequest)
            {
                string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
                _logger.LogDebug("Admin[{0}] {1}", isAdminrequest, message);
            }

            public void Error(string formatString, Exception exception)
            {
                _logger.LogError(formatString, exception.Message);
            }
        }

        public WireMockService(ILogger<WireMockService> logger, IOptions<WireMockServerSettings> settings)
        {
            _logger = logger;
            _settings = settings.Value;

            _settings.Logger = new Logger(logger);
        }

        public void Start()
        {
            _logger.LogInformation("WireMock.Net server starting");

            _server = WireMockServer.Start(_settings);

            CreateFakeHttpApis();

            _logger.LogInformation($"WireMock.Net server settings {JsonConvert.SerializeObject(_settings)}");
        }


        private void CreateFakeHttpApis()
        {
            var fakeBreachedInfo = new List<BreachedInfo>
            {
               new BreachedInfo{
                   Name="adobe",
                   Domain="www.adobe.com",
                   BreachDate=new DateTime(2021,01,01),
                   AddedDate=new DateTime(2021,01,02),
                   ModifiedDate=new DateTime(2021,01,02),
                   Title="Adobe"
               },
               new BreachedInfo{
                   Name="telegram",
                   Domain="www.telegram.com",
                   BreachDate=new DateTime(2021,01,01),
                   AddedDate=new DateTime(2021,01,02),
                   ModifiedDate=new DateTime(2021,01,02),
                   Title="Telegram"
               },
               new BreachedInfo{
                   Name="facebook",
                   Domain="www.facebook.com",
                   BreachDate=new DateTime(2021,01,01),
                   AddedDate=new DateTime(2021,01,02),
                   ModifiedDate=new DateTime(2021,01,02),
                   Title="FaceBook"
               }

            };

            IRequestBuilder BuildRequest(string accountValue)
            {
                return Request.Create()
                              .WithPath($"/api/v3/breachedaccount/{accountValue}")
                              .WithHeader("hibp-api-key", "13456")
                              .UsingGet();
            }
            IResponseBuilder BuildResponse(string accountValue)
            {
                return Response
                .Create()
                .WithBodyAsJson(fakeBreachedInfo.Find(item => item.Name.Equals(accountValue, StringComparison.CurrentCultureIgnoreCase)));
            }

            _server.Given(BuildRequest("adobe")).RespondWith(BuildResponse("adobe"));
            _server.Given(BuildRequest("facebook")).RespondWith(BuildResponse("facebook"));
            _server.Given(BuildRequest("telegram")).RespondWith(BuildResponse("telegram"));
            _server.Given(BuildRequest("gmail")).RespondWith(Response.Create().WithBodyAsJson(new object()));
        }


        public void Stop()
        {
            _logger.LogInformation("WireMock.Net server stopping");
            _server?.Stop();
        }
    }
}