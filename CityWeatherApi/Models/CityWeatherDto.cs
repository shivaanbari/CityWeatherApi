namespace CityWeatherApi.Models
{
    public class CityWeatherDto
    {
        public string City { get; set; }
        public double TemperatureCelsius { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AirQualityIndex { get; set; }
        public Dictionary<string, double> Pollutants { get; set; }
    }
}
