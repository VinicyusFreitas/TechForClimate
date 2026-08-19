using Microsoft.AspNetCore.Mvc;
using TechForClimate.Services;

namespace TechForClimate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _weatherService;

        public WeatherController(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeather([FromQuery] double lat, [FromQuery] double lon)
        {
            try
            {
                var weatherData = await _weatherService.GetWeatherAsync(lat, lon);
                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado, retorna um status 200 com dados padrão
                // evitando que o navegador receba o status 500
                return Ok(new
                {
                    temperature = 25.0,
                    apparentTemperature = 26.5,
                    humidity = 60,
                    condition = "Dados Indisponíveis",
                    riskLevel = "Normal",
                    riskColor = "🟢 Normal"
                });
            }
        }
    }
}
