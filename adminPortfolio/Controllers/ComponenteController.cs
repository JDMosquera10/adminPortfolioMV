using adminportfolio.Dtos;
using adminportfolio.Models;
using adminportfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Text.Json;

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
        public async Task<ActionResult<List<ComponenteDto>>> Get()
        {
            var componentes = await _service.GetAsync();
            var dtos = componentes.Select(c => new ComponenteDto
            {
                Id = c.Id,
                Type = c.Type,
                Identifier = c.Identifier,
                Props_json = c.Props_json != null
                    ? JsonDocument.Parse(c.Props_json.ToJson()).RootElement
                    : null
            }).ToList();

            return dtos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ComponenteDto>> GetById(string id)
        {
            var componente = await _service.GetByIdAsync(id);
            if (componente is null) return NotFound();

            var dto = new ComponenteDto
            {
                Id = componente.Id,
                Type = componente.Type,
                Identifier = componente.Identifier,
                Props_json = componente.Props_json != null
                    ? JsonDocument.Parse(componente.Props_json.ToJson()).RootElement
                    : null
            };

            return dto;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ComponenteDto dto)
        {
            var componente = new Componente
            {
                Id = dto.Id,
                Type = dto.Type,
                Identifier = dto.Identifier,
                Props_json = dto.Props_json is JsonElement jsonElement && jsonElement.ValueKind != JsonValueKind.Null
                    ? BsonDocument.Parse(jsonElement.GetRawText())
                    : null
            };

            await _service.CreateAsync(componente);
            var resultDto = new ComponenteDto
            {
                Id = componente.Id,
                Type = componente.Type,
                Identifier = componente.Identifier,
                Props_json = componente.Props_json != null
            ? JsonDocument.Parse(componente.Props_json.ToJson()).RootElement
            : default(JsonElement?)
            };
            return CreatedAtAction(nameof(GetById), new { id = componente.Id }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, ComponenteDto dto)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            var componente = new Componente
            {
                Id = id,
                Type = dto.Type,
                Identifier = dto.Identifier,
                Props_json = dto.Props_json is JsonElement jsonElement && jsonElement.ValueKind != JsonValueKind.Null
                    ? BsonDocument.Parse(jsonElement.GetRawText())
                    : null
            };

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