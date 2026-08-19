using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using TechForClimate.Models;

namespace TechForClimate.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherData> GetWeatherAsync(double latitude, double longitude)
        {
            // Converte latitude e longitude para string usando ponto decimal (invariante)
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&current=temperature_2m,relative_humidity_2m,apparent_temperature,weather_code";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);
            var current = doc.RootElement.GetProperty("current");

            // Leitura segura garantindo conversão numérica
            double temp = current.GetProperty("temperature_2m").GetDouble();
            double apparentTemp = current.GetProperty("apparent_temperature").GetDouble();
            int humidity = current.GetProperty("relative_humidity_2m").GetInt32();

            var weatherData = new WeatherData
            {
                Temperature = temp,
                ApparentTemperature = apparentTemp,
                Humidity = humidity,
                Condition = "Ensolarado / Parcialmente Nublado"
            };

            // Regra de Negócio: Classificação de Risco por Calor
            if (apparentTemp < 28)
            {
                weatherData.RiskLevel = "Normal";
                weatherData.RiskColor = "🟢 Normal";
            }
            else if (apparentTemp >= 28 && apparentTemp < 33)
            {
                weatherData.RiskLevel = "Atenção";
                weatherData.RiskColor = "🟡 Atenção";
            }
            else if (apparentTemp >= 33 && apparentTemp < 38)
            {
                weatherData.RiskLevel = "Alerta";
                weatherData.RiskColor = "🟠 Alerta";
            }
            else
            {
                weatherData.RiskLevel = "Risco Elevado";
                weatherData.RiskColor = "🔴 Risco elevado";
            }

            return weatherData;
        }
    }
}