using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;
using FluentAssertions;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string?> GetCityWeatherAsync(string city)
    {
        var baseUrl = _config["OpenWeatherMap:BaseUrl"];
        var apiKey = _config["OpenWeatherMap:ApiKey"];
        var weatherUrl = $"{baseUrl}/weather?q={city}&appid={apiKey}";

        var response = await _httpClient.GetAsync(weatherUrl);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadAsStringAsync();
    }
}

public class WeatherServiceTests
{
    [Fact]
    public async Task GetCityWeatherAsync_ReturnsWeatherJson_WhenSuccessful()
    {
        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(c => c["OpenWeatherMap:BaseUrl"]).Returns("https://api.openweathermap.org/data/2.5");
        mockConfig.Setup(c => c["OpenWeatherMap:ApiKey"]).Returns("test-key");

        var mockHandler = new Mock<HttpMessageHandler>();

        mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(r => r.RequestUri!.AbsoluteUri.Contains("weather")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("{\"temp\": 22}")
            });

        var client = new HttpClient(mockHandler.Object);
        var service = new WeatherService(client, mockConfig.Object);

        var result = await service.GetCityWeatherAsync("Tehran");

        result.Should().Contain("22");
    }
}
