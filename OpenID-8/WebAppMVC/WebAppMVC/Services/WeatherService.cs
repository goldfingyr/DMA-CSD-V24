using System.Net.Http.Headers;

namespace WebAppMVC.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("WeatherApiClient");
        }

        public async Task<string> GetProtectedDataAsync(string access_token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await _httpClient.GetAsync("WeatherForecast");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
