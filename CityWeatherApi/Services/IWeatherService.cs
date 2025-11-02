using CityWeatherApi.Models;
using System.Threading.Tasks;

namespace CityWeatherApi.Services
{
    public interface IWeatherService
    {
        Task<CityWeatherDto?> GetCityWeatherAsync(string city);
    }
}
