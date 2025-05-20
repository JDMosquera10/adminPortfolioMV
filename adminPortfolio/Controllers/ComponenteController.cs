using adminportfolio.Models;
using adminportfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adminportfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComponenteController : ControllerBase
    {
        private readonly ComponenteService _service;

        public ComponenteController(ComponenteService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Componente>>> Get() =>
            await _service.GetAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Componente>> GetById(string id)
        {
            var componente = await _service.GetByIdAsync(id);
            if (componente is null) return NotFound();
            return componente;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Componente componente)
        {
            await _service.CreateAsync(componente);
            return CreatedAtAction(nameof(GetById), new { id = componente.Id }, componente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Componente componente)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            componente.Id = id;
            await _service.UpdateAsync(id, componente);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}