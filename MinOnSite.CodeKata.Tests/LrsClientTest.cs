using Microsoft.Extensions.Logging;
using MindOnSite.CodeKata.Implementations;
using Moq;

namespace MinOnSite.CodeKata.Tests
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
            var logger = Mock.Of<ILoggerFactory>();
            var service = new LrsService(logger);
            service.GetStatementAsync("myId").ConfigureAwait(false);
            Assert.NotNull(service.GetStatementResult);          
        }

        [Test]
        public async Task Test2()
        {
            var logger = Mock.Of<ILoggerFactory>();
            var service = new LrsService(logger);
            var result = await service.SearchStatementsAsync(SearchTypes.TypeA, "mytext");
            Assert.NotNull(result);
        }
    }
}