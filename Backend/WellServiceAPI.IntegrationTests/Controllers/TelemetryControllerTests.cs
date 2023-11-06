using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;
using WellServiceAPI.Shared.Request;

namespace WellServiceAPI.IntegrationTests.Controllers
{
    [TestFixture]
    public class TelemetryControllerTests
    {
        [Test]
        public async Task Post_ReceiveTelemetryData_ShouldOkResult()
        {
            var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var random = new Random();
            var telemetries = new List<TelemetryData>();
            for (int i = 0; i < 15; i++)
            {
                telemetries.Add(new TelemetryData { DateTime = new DateTime(), Depth = random.NextSingle(), WellId = random.Next(1, 5) });
            }

            using HttpResponseMessage response = await client.PostAsJsonAsync<IEnumerable<TelemetryData>>("api/v1/telemetry/all", telemetries);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Post_ReceiveTelemetryData_ShouldBadRequestResult()
        {
            var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var telemetries = new List<TelemetryData>();

            using HttpResponseMessage response = await client.PostAsJsonAsync<IEnumerable<TelemetryData>>("api/v1/telemetry/all", telemetries);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}
