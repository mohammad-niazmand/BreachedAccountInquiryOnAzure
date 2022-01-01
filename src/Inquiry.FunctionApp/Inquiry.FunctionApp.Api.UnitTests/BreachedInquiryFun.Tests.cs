using Inquiry.Func.Functions;
using Inquiry.Func.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Moq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inquiry.Func.UnitTests
{
    public class BreachedInquiryFunTests
    {
        private readonly Mock<IAsyncCollector<BreachInquiryRequest>> _mockverifyBreachRequestQueue = new();
        private readonly Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger = new();

        [Fact]
        public async Task Breaches_GivenPayloadIsNotValid_ReturnBadRequest()
        {
            //Arrange
            var breachedInquiryFun = new BreachedInquiryFun();
            var cancellationToken = new CancellationToken();
            var mockHttpRequest = CreateMockRequest(new { acont = "account" });

            //Act
            var actionResult = await breachedInquiryFun.Run(mockHttpRequest.Object, _mockverifyBreachRequestQueue.Object,
                                                             cancellationToken, _mockLogger.Object);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Breaches_GivenAccountIsEmpty_ReturnBadRequest()
        {
            //Arrange
            var breachedInquiryFun = new BreachedInquiryFun();
            var cancellationToken = new CancellationToken();
            const string emptyAccountName = " ";
            var mockHttpRequest = CreateMockRequest(new BreachInquiryRequest { Account = emptyAccountName });

            //Act
            var actionResult = await breachedInquiryFun.Run(mockHttpRequest.Object, _mockverifyBreachRequestQueue.Object,
                                                             cancellationToken, _mockLogger.Object);

            //Assert
            Assert.IsType<BadRequestObjectResult>(actionResult);
        }

        [Fact]
        public async Task Breaches_GivenAccountIsValid_InsertRequestIntoQueue()
        {
            //Arrange
            var breachedInquiryFun = new BreachedInquiryFun();
            var cancellationToken = new CancellationToken();
            var mockHttpRequest = CreateMockRequest(new BreachInquiryRequest { Account = "Account" });
            _mockverifyBreachRequestQueue.Setup(call => call.AddAsync(It.IsAny<BreachInquiryRequest>(), cancellationToken))
                .Verifiable();

            //Act
            var actionResult = await breachedInquiryFun.Run(mockHttpRequest.Object, _mockverifyBreachRequestQueue.Object,
                                                             cancellationToken, _mockLogger.Object);

            //Assert
            _mockverifyBreachRequestQueue.VerifyAll();
        }

        [Fact]
        public async Task Breaches_GivenAccountIsValid_ReturnOk()
        {
            //Arrange
            var breachedInquiryFun = new BreachedInquiryFun();
            var cancellationToken = new CancellationToken();
            var mockHttpRequest = CreateMockRequest(new BreachInquiryRequest { Account = "Account" });

            //Act
            var actionResult = await breachedInquiryFun.Run(mockHttpRequest.Object, _mockverifyBreachRequestQueue.Object,
                                                             cancellationToken, _mockLogger.Object);

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
        }

        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);

            var json = JsonConvert.SerializeObject(body);

            sw.Write(json);
            sw.Flush();

            ms.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(ms);

            return mockRequest;
        }
    }
}
