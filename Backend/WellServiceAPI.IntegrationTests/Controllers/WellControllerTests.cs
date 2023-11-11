using NUnit.Framework;
using System.Net;
using System.Text.Json;
using WellServiceAPI.Shared.Response.Well;

namespace WellServiceAPI.IntegrationTests.Controllers
{
    [TestFixture]
    public class WellControllerTests : StartUpTestsBase
    {
        [Test]
        public async Task Get_GetWellById_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/1");

            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.Not.Empty);
            Assert.That(content.ToUpper(), Is.EqualTo("deep"));
        }

        [Test]
        public async Task Get_GetWellById_ShouldNotFoundResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/0");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Get_GetWellsByCompanyName_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/company/black gold");

            var content = await response.Content.ReadAsStringAsync();

            var companies = JsonSerializer.Deserialize<IEnumerable<string>>(content, jsonSerializerOptions);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.Not.Empty);
            Assert.That(companies!.Count(), Is.EqualTo(3));
            Assert.That(companies, Is.EqualTo(new string[] { "Deep", "Bryl", "Kirovskaya borehole" }));
        }

        [Test]
        public async Task Get_GetWellsByCompanyName_ShouldNotFoundResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/company/none");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Get_AllActiveWells_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/active/all");

            var content = await response.Content.ReadAsStringAsync();

            var wellsWithConstractors = JsonSerializer.Deserialize<IEnumerable<WellsWithContractors>>(content, jsonSerializerOptions);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(wellsWithConstractors, Is.Not.Empty);
            Assert.That(wellsWithConstractors.Count(), Is.AtLeast(0));
        }

        [Test]
        public async Task Get_GetActiveWellsById_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/active/2");

            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.Not.Empty);
            Assert.That(content, Is.EqualTo("Bryl"));
        }

        [Test]
        public async Task Get_GetActiveWellsById_ShouldNotFoundResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/active/0");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Get_GetActiveWellsByCompanyName_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/active/company/black gold");

            var content = await response.Content.ReadAsStringAsync();

            var wells = JsonSerializer.Deserialize<IEnumerable<string>>(content, jsonSerializerOptions);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(content, Is.Not.Null);
            Assert.That(content, Is.Not.Empty);
            Assert.That(wells!.Count(), Is.AtLeast(0));
            Assert.That(wells, Is.EqualTo(new string[] { "Deep", "Bryl", "Kirovskaya borehole" }));
        }

        [Test]
        public async Task Get_GetActiveWellsByCompanyName_ShouldNotFoundResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/active/company/none");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Get_GetTotalDepthByWellId_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/depth/1?fromDate=2023-01-01 23:22&toDate=2023-10-01 23:22");

            var content = await response.Content.ReadAsStringAsync();

            var wellDepth = JsonSerializer.Deserialize<double>(content, jsonSerializerOptions);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(wellDepth, Is.AtLeast(0));
        }

        [Test]
        public async Task Get_GetTotalDepthByWellId_ShouldNotFoundResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/depth/0?fromDate=2023-01-01 23:22&toDate=2023-10-01 23:22");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Get_GetTotalDepthByCompanyId_ShouldCorrectResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/depth/company/1?fromDate=2023-01-01 23:22&toDate=2023-10-01 23:22");

            var content = await response.Content.ReadAsStringAsync();

            var totalDepthWells = JsonSerializer.Deserialize<List<TotalDepthWells>>(content, jsonSerializerOptions);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(totalDepthWells!.Count, Is.EqualTo(3));
            Assert.That(totalDepthWells[0].WellName, Is.EqualTo("Deep"));
            Assert.That(totalDepthWells[0].Score, Is.AtLeast(0));
            Assert.That(totalDepthWells[1].WellName, Is.EqualTo("Bryl"));
            Assert.That(totalDepthWells[1].Score, Is.AtLeast(0));
        }

        [Test]
        public async Task Get_GetTotalDepthByCompanyId_ShouldNotFoundResult()
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("api/v1/well/depth/company/0?fromDate=2023-01-01 23:22&toDate=2023-10-01 23:22");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}
