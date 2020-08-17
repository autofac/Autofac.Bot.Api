using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Autofac.Bot.Api.Controllers.Presentation;
using Autofac.Bot.Api.Controllers.Tests.Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Autofac.Bot.Api.Controllers.Tests
{
    public class BenchmarkControllerTests : IClassFixture<WebApplicationFactoryFixture>
    {
        private const string BaseAddress = "api/v1/benchmarks";
        private const string AutofacRepoUrl = "https://github.com/Autofac/Autofac.git";
        private const string JsonMediaType = "application/json";


        private readonly WebApplicationFactoryFixture _webApplicationFactoryFixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public BenchmarkControllerTests(WebApplicationFactoryFixture webApplicationFactoryFixture,
            ITestOutputHelper testOutputHelper)
        {
            _webApplicationFactoryFixture = webApplicationFactoryFixture;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task ExecuteBenchmarkAsync_ValidRequest_HttpStatusCodeOk()
        {
            const string childScopeBenchmark = "ChildScopeResolveBenchmark";
            var client = _webApplicationFactoryFixture.Server.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(5);
            var request = new BenchmarkRequestDto(childScopeBenchmark, true,
                new RepositoryDto("develop", AutofacRepoUrl),
                new RepositoryDto("develop", AutofacRepoUrl));
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, JsonMediaType);

            var benchmarkResponse = await client.PostAsync(BaseAddress, content);

            var output = await benchmarkResponse.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine(output);
            benchmarkResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ExecuteBenchmarkAsync_InvalidRef_HttpStatusCodeConflict()
        {
            const string childScopeBenchmark = "ChildScopeResolveBenchmark";
            var client = _webApplicationFactoryFixture.Server.CreateClient();
            var request = new BenchmarkRequestDto(childScopeBenchmark, true,
                new RepositoryDto(Guid.NewGuid().ToString().Replace("-", ""), AutofacRepoUrl),
                new RepositoryDto("develop", AutofacRepoUrl));
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, JsonMediaType);

            var benchmarkResponse = await client.PostAsync(BaseAddress, content);

            var output = await benchmarkResponse.Content.ReadAsStringAsync();
            _testOutputHelper.WriteLine(output);
            benchmarkResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }
    }
}