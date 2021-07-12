using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WideWorldImporters.Api.IntegrationTests.HealthTests
{
    public sealed class HealthCheckTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        public HealthCheckTests(WebApplicationFactory<Startup> factory)
        {
            _httpClient = factory.CreateDefaultClient();
        }

        private readonly HttpClient _httpClient;

        /// <summary>
        ///     Verify the API runs and a health check returns success
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _httpClient.GetAsync("/healthcheck");

            response.EnsureSuccessStatusCode();
        }
    }
}