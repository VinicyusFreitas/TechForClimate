using Microsoft.AspNetCore.Mvc;
using TechForClimate.Models;
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
        public async Task<ActionResult<WeatherData>> Get([FromQuery] double lat, [FromQuery] double lon)
        {
            try
            {
                var data = await _weatherService.GetWeatherAsync(lat, lon);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao buscar dados meteorológicos", error = ex.Message });
            }
        }
    }
}