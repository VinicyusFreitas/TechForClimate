using Microsoft.AspNetCore.Mvc;
using TechForClimate.Models;
using TechForClimate.Services;

namespace TechForClimate.Controllers
{
    [ApiController]
    [Route("api/occurrences")]
    public class OccurrencesController : ControllerBase
    {
        private readonly OccurrenceService _service;

        public OccurrencesController(OccurrenceService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        [HttpPost]
        public IActionResult Create([FromBody] Occurrence occurrence)
        {
            _service.Add(occurrence);
            return Ok(occurrence);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _service.Delete(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}