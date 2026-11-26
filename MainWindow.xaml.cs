using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace WeatherApp
{

    public partial class MainWindow : Window
    {

        private static readonly HttpClient httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += async (s, e) => await UpdateUI("Graz");


        }

        public async Task UpdateUI(string city)
        {
            try
            {
                GeoLocation local = await GetLocation(city);
                if ( local == null)
                {
                    labelInfo.Content = "Stadt nicht gefunden";
                    return;
                }

                WeatherResponse weather = await GetWeatherData(local);
                int code = weather.Current.WeatherCode;

                backgroundImage.ImageSource = new BitmapImage(new Uri("Images/" + GetImageName(code), UriKind.Relative));
                labelTemperature.Content = weather.Current.Temperature.ToString("F1") + "°C";
                labelInfo.Content = local.Name + ": " + GetDescription(code);
            }

            catch (HttpRequestException)
            {
                labelInfo.Content = "Keine Verbindung zum Wetterdienst";
            }
        }


        private async Task<GeoLocation> GetLocation(string city)
        {
            string url = "https://geocoding-api.open-meteo.com/v1/search?name=" + Uri.EscapeDataString(city) + "&count=1&language=de";
            string json = await httpClient.GetStringAsync(url);
            GeoResponse geo = JsonConvert.DeserializeObject<GeoResponse>(json);
            return geo.Results?.FirstOrDefault();
        }

        private async Task<WeatherResponse> GetWeatherData(GeoLocation local)
        {
            string lat = local.Latitude.ToString(CultureInfo.InvariantCulture);
            string lon = local.Longitude.ToString(CultureInfo.InvariantCulture);
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,weather_code";
            string json = await httpClient.GetStringAsync(url);
            return JsonConvert.DeserializeObject<WeatherResponse>(json);
        }

        private string GetImageName(int code)
        {
            if (code <= 1) return "sun.png";
            if (code == 2 || code == 3 || code == 45 || code == 48) return "cloud.png";
            if ((code >= 71 && code <= 77) || code == 85 || code == 86) return "snow.png";
            return "rain.png";
        }
        private string GetDescription(int code)
        {
            if (code == 0) return "Klar";
            if (code == 1) return "Überwiegend klar";
            if (code == 2) return "Teilweise bewölkt";
            if (code == 3) return "Bedeckt";
            if (code == 45 || code == 48) return "Nebel";
            if (code >= 51 && code <= 57) return "Nieselregen";
            if (code >= 61 && code <= 67) return "Regen";
            if (code >= 71 && code <= 77) return "Schneefall";
            if (code >= 80 && code <= 82) return "Regenschauer";
            if (code == 85 || code == 86) return "Schneeschauer";
            if (code >= 95) return "Gewitter";
            return "Unbekannt";
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await UpdateUI(textBoxQuery.Text);
        }
    }
}