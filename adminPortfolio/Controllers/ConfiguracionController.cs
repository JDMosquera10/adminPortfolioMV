using adminportfolio.Models;
using adminportfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adminportfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todas las rutas del controlador
    public class ConfiguracionController : ControllerBase
    {
        private readonly ConfiguracionService _service;

        public ConfiguracionController(ConfiguracionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Configuracion>>> Get() =>
            await _service.GetAsync();

        [HttpGet("{userId}")]
        public async Task<ActionResult<Configuracion>> GetByUserId(string userId)
        {
            var config = await _service.GetByUserIdAsync(userId);
            if (config is null) return NotFound();
            return config;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Configuracion configuracion)
        {
            await _service.CreateAsync(configuracion);
            return CreatedAtAction(nameof(GetByUserId), new { userId = configuracion.UserId }, configuracion);
        }

        [HttpPost("{userId}/sections")]
        public async Task<IActionResult> AddSection(string userId, [FromBody] Section section)
        {
            var success = await _service.AddSectionAsync(userId, section);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Update(string userId, Configuracion configuracion)
        {
            var existing = await _service.GetByUserIdAsync(userId);
            if (existing is null) return NotFound();

            configuracion.UserId = userId;
            await _service.UpdateAsync(userId, configuracion);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            var existing = await _service.GetByUserIdAsync(userId);
            if (existing is null) return NotFound();

            await _service.DeleteAsync(userId);
            return NoContent();
        }
    }
}