using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MindOnSite.CodeKata.Implementations.Services;
using Moq;

namespace MinOnSite.CodeKata.Tests.Implementations.Services
{
	public class Tests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Test1()
        {
            var service = new LrsService(NullLoggerFactory.Instance);
            service.GetStatementAsync("myId").ConfigureAwait(false);
            Assert.NotNull(service.GetStatementResultInCache);
        }

        [Test]
        public async Task Test2()
        {
            var service = new LrsService(NullLoggerFactory.Instance);
            var result = await service.SearchStatementsAsync(SearchTypes.TypeA, "mytext");
            Assert.NotNull(result);
        }
    }
}