using adminportfolio.Dtos;
using adminportfolio.Models;
using adminportfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System.Text.Json;

namespace adminportfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConfiguracionController : ControllerBase
    {
        private readonly ConfiguracionService _service;

        public ConfiguracionController(ConfiguracionService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<ConfigurationDto>>> Get()
        {
            var configuraciones = await _service.GetAsync();
            if (configuraciones == null || !configuraciones.Any())
            {
                return NotFound();
            }

            var configuracionDtos = configuraciones.Select(configuracion => new ConfigurationDto
            {
                UserId = configuracion.UserId,
                Sections = configuracion.Sections.Select(s => new SectionsDto
                {
                    Order = s.Order,
                    fullname = s.Identifier, 
                    Type = s.Type,
                    Title = s.Title,
                    Componente_identifier = s.Componente_identifier,
                    RenderClient = s.RenderClient,
                    Content = s.Content?.FirstOrDefault() != null ? new ContentDto
                    {
                        Position = s.Content.FirstOrDefault().Position,
                        IsHidden = s.Content.FirstOrDefault().IsHidden,
                        Stuffed = s.Content.FirstOrDefault().Stuffed,
                        componenteIdentifier = s.Content.FirstOrDefault().ComponenteIdentifier
                    } : null, // Fix: Properly map Content to ContentDto
                    IsNavbar = s.IsNavbar
                }).ToList()
            }).ToList();

            return Ok(configuracionDtos);
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<Configuracion>> GetByUserId(string userId)
        {
            var config = await _service.GetByUserIdAsync(userId);
            if (config is null) return NotFound();
            return config;
        }

        [HttpGet("getconfig/{userId}")]
        [AllowAnonymous]
        public async Task<ActionResult<ConfiguracionGeneralDto>> GetConfigData(string userId)
        {
            var config = await _service.GetConfigDataWithComponentes(userId);
            if (config is null)
            {
                return NotFound();
            }

            var configuracionDto = new ConfiguracionGeneralDto
            {
                UserId = config.UserId,
                Sections = config.Sections.Select(s => new SectionsDtoGeneral
                {
                    Order = s.Order,
                    fullname = s.Identifier,
                    Type = s.Type,
                    Title = s.Title,
                    Componente_identifier = s.Componente_identifier,
                    contentData = s.ContentData != null 
                        ? JsonDocument.Parse(s.ContentData.ToJson()).RootElement 
                        : null,
                    RenderClient = s.RenderClient,
                    Contents = s.Content?.Select(c => new ContentDtoGeneral
                    {
                        Position = c.Position,
                        IsHidden = c.IsHidden,
                        contentData = c.ContentData != null 
                            ? JsonDocument.Parse(c.ContentData.ToJson()).RootElement 
                            : null,
                        Stuffed = c.Stuffed,
                        componenteIdentifier = c.ComponenteIdentifier
                    }).ToList(),
                    IsNavbar = s.IsNavbar
                }).ToList()
            };

            return Ok(configuracionDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ConfigurationDto dto)
        {
            // Map SectionsDto to List<Section>
            var sections = dto.Sections.Select(s => new Section
            {
                Order = s.Order,
                Identifier = s.fullname, // Assuming fullname maps to Identifier
                Type = s.Type,
                ContentData = null, // Fix: Convert JsonElement? to string before parsing
                Title = s.Title,
                Componente_identifier = s.Componente_identifier,
                RenderClient = s.RenderClient,
                Content = s.Content != null ? new List<Content> // Fix: Ensure Content is mapped to a List<Content>
                {
                    new Content
                    {
                        Position = s.Content.Position,
                        IsHidden = s.Content.IsHidden,
                        Stuffed = s.Content.Stuffed,
                        ComponenteIdentifier = s.Content.componenteIdentifier
                    }
                } : null,
                IsNavbar = s.IsNavbar
            }).ToList();

            var configuracion = new Configuracion
            {
                UserId = dto.UserId,
                Sections = sections
            };

            await _service.CreateAsync(configuracion);

            // Map List<Section> back to List<SectionsDto> for the response
            var sectionsDto = configuracion.Sections.Select(s => new SectionsDto
            {
                Order = s.Order,
                fullname = s.Identifier, // Assuming Identifier maps back to fullname
                Type = s.Type,
                Title = s.Title,
                Componente_identifier = s.Componente_identifier,
                RenderClient = s.RenderClient,
                Content = s.Content?.FirstOrDefault() != null ? new ContentDto
                {
                    Position = s.Content.FirstOrDefault().Position,
                    IsHidden = s.Content.FirstOrDefault().IsHidden,
                    Stuffed = s.Content.FirstOrDefault().Stuffed,
                    componenteIdentifier = s.Content.FirstOrDefault().ComponenteIdentifier
                } : null, // Fix: Properly map Content to ContentDto
                IsNavbar = s.IsNavbar
            }).ToList();

            var configuracionResult = new ConfigurationDto
            {
                UserId = configuracion.UserId,
                Sections = sectionsDto
            };

            return CreatedAtAction(nameof(GetByUserId), new { userId = configuracion.UserId }, configuracionResult);
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
            configuracion.Id = existing.Id;
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