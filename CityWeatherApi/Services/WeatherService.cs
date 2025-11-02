using CityWeatherApi.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace CityWeatherApi.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WeatherService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<CityWeatherDto?> GetCityWeatherAsync(string city)
        {
            var baseUrl = _config["OpenWeatherMap:BaseUrl"];
            var apiKey = _config["OpenWeatherMap:ApiKey"];

            var weatherUrl = $"{baseUrl}/weather?q={city}&appid={apiKey}&units=metric";
            var weatherResponse = await _httpClient.GetAsync(weatherUrl);
            if (!weatherResponse.IsSuccessStatusCode) return null;

            var weatherJson = await weatherResponse.Content.ReadAsStringAsync();
            var weatherData = JsonDocument.Parse(weatherJson).RootElement;

            var lat = weatherData.GetProperty("coord").GetProperty("lat").GetDouble();
            var lon = weatherData.GetProperty("coord").GetProperty("lon").GetDouble();
            var temp = weatherData.GetProperty("main").GetProperty("temp").GetDouble();
            var humidity = weatherData.GetProperty("main").GetProperty("humidity").GetInt32();
            var wind = weatherData.GetProperty("wind").GetProperty("speed").GetDouble();

            var airUrl = $"{baseUrl}/air_pollution?lat={lat}&lon={lon}&appid={apiKey}";
            var airResponse = await _httpClient.GetAsync(airUrl);
            if (!airResponse.IsSuccessStatusCode) return null;

            var airJson = await airResponse.Content.ReadAsStringAsync();
            var airData = JsonDocument.Parse(airJson).RootElement;

            var aqi = airData.GetProperty("list")[0].GetProperty("main").GetProperty("aqi").GetInt32();
            var components = airData.GetProperty("list")[0].GetProperty("components");

            var pollutants = new Dictionary<string, double>();
            foreach (var prop in components.EnumerateObject())
                pollutants[prop.Name] = prop.Value.GetDouble();

            return new CityWeatherDto
            {
                City = city,
                TemperatureCelsius = temp,
                Humidity = humidity,
                WindSpeed = wind,
                Latitude = lat,
                Longitude = lon,
                AirQualityIndex = aqi,
                Pollutants = pollutants
            };
        }
    }
}
