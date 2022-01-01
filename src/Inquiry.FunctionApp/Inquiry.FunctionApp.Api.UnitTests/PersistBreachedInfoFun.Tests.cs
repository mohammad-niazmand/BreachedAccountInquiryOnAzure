using Inquiry.Func.Abstractions;
using Inquiry.Func.Functions;
using Inquiry.Func.Models;
using Microsoft.Azure.WebJobs;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Inquiry.Func.UnitTests
{
    public class PersistBreachedInfoFunTests
    {
        private readonly Mock<IBreachInquiryService> _mockBreachService = new();
        private readonly Mock<IAsyncCollector<BreachedInfo>> _mockbreachedInfoCosmosDB = new();
        private readonly Mock<Microsoft.Extensions.Logging.ILogger> _mockLogger = new();

        [Fact]
        public async Task ProcessRequests_ReadAccountIsEmpty_ThrowArgumentExceptionAsync()
        {
            //Arrange
            const string emptyAccountName = " ";
            var invalildItem = new BreachInquiryRequest { Account = emptyAccountName };
            var persistBreachedInfoFun = new PersistBreachedInfoFun(_mockBreachService.Object);


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                   await persistBreachedInfoFun.Run(invalildItem, _mockbreachedInfoCosmosDB.Object, _mockLogger.Object));
        }

        [Fact]
        public async Task ProcessRequests_GivenAccountHasBreachedInfo_ShouldSaveInfoInDB()
        {
            //Arrange
            var valildItem = new BreachInquiryRequest { Account = "account" };

            _mockBreachService
                .Setup(call => call.ValidateAccountAsync(It.IsAny<string>()))
                .ReturnsAsync((true, new BreachedInfo { Name = "name", Title = "title", Domain = "domain" }));
            var persistBreachedInfoFun = new PersistBreachedInfoFun(_mockBreachService.Object);

            _mockbreachedInfoCosmosDB
                .Setup(call => call.AddAsync(It.IsAny<BreachedInfo>(), It.IsAny<CancellationToken>())).Verifiable();

            //Act
            await persistBreachedInfoFun.Run(valildItem, _mockbreachedInfoCosmosDB.Object, _mockLogger.Object);

            //Assert
            _mockbreachedInfoCosmosDB.VerifyAll();
        }
    }
}
