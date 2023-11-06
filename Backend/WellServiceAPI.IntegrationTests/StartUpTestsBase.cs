using System.Text.Json;

namespace WellServiceAPI.IntegrationTests
{
    public class StartUpTestsBase
    {
        protected readonly CustomWebApplicationFactory _webApplicationFactory;
        protected readonly HttpClient _httpClient;
        protected readonly JsonSerializerOptions jsonSerializerOptions;

        public StartUpTestsBase()
        {
            _webApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _webApplicationFactory.CreateClient();
            jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }
}
