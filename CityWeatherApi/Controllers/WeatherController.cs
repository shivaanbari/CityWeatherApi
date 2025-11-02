using CityWeatherApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityWeatherApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{city}")]
        public async Task<IActionResult> GetCityWeather(string city)
        {
            var result = await _weatherService.GetCityWeatherAsync(city);
            if (result == null)
                return NotFound($"City '{city}' not found or API error.");

            return Ok(result);
        }
    }
}
