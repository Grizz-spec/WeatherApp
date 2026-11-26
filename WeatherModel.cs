using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WeatherApp
{
    class GeoResponse
    {
        public List<GeoLocation> Results {  get; set; }
    }

    class GeoLocation
    {
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    class WeatherResponse
    {
        public CurrentWeather Current { get; set; }
    }

    class CurrentWeather
    {
        [JsonProperty("temperature_2m")]
        public double Temperature { get; set; }

        [JsonProperty("weather_code")]
        public int WeatherCode { get; set; }
    }
}
