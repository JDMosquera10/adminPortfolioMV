using adminportfolio.Dtos;
using adminportfolio.Models;
using adminportfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using System.Security.Claims;
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
        [Authorize(Roles = "Admin,User")]
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
                    Content = s.Content?.Select(c => new ContentDto
                    {
                        Position = c.Position,
                        IsHidden = c.IsHidden, 
                        Stuffed = c.Stuffed,
                        componenteIdentifier = c.ComponenteIdentifier,
                        type = c.type
                    }).ToList(), // Fix: Properly map Content to ContentDto
                    IsNavbar = s.IsNavbar
                }).ToList()
            }).ToList();

            return Ok(configuracionDtos);
        }

        [HttpGet("{userId}")]
        [AllowAnonymous]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<ConfigurationDto>> GetByUserId(string userId)
        {
            var config = await _service.GetByUserIdAsync(userId);
            if (config is null) return NotFound();

            var configuracionDtos = new ConfigurationDto
            {
                UserId = config.UserId,
                Sections = config.Sections.Select(s => new SectionsDto
                {
                    Order = s.Order,
                    fullname = s.Identifier,
                    Type = s.Type,
                    Title = s.Title,
                    Componente_identifier = s.Componente_identifier,
                    RenderClient = s.RenderClient,
                    Content = s.Content?.Select(c => new ContentDto
                    {
                        Position = c.Position,
                        IsHidden = c.IsHidden,
                        Stuffed = c.Stuffed,
                        componenteIdentifier = c.ComponenteIdentifier,
                        type = c.type
                    }).ToList(), // Fix: Properly map Content to ContentDto
                    IsNavbar = s.IsNavbar
                }).ToList()
            };
            return Ok(configuracionDtos);
        }

        [HttpGet("getconfig/{userId}")]
        [AllowAnonymous]
        [Authorize(Roles = "Admin,User")]
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
                        componenteIdentifier = c.ComponenteIdentifier,
                        type = c.type
                    }).ToList(),
                    IsNavbar = s.IsNavbar
                }).ToList()
            };

            return Ok(configuracionDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ConfigurationDto dto)
        {
            try
            {
                // Obtener el ID del usuario del token
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Usuario no autenticado" });
            }
            // Asignar el ID del usuario autenticado a la configuración
            dto.UserId = userId;
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
                Content = s.Content?.Select(c => new Content // Fix: Ensure Content is mapped to a List<Content>
                {
                    Position = c.Position,
                    IsHidden = c.IsHidden,
                    Stuffed = c.Stuffed,
                    ComponenteIdentifier = c.componenteIdentifier,
                    type = c.type
                }).ToList(),
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
                Content = s.Content?.Select(c => new ContentDto
                {
                    Position = c.Position,
                    IsHidden = c.IsHidden,
                    Stuffed = c.Stuffed,
                    componenteIdentifier = c.ComponenteIdentifier,
                    type = c.type
                }).ToList(),
                IsNavbar = s.IsNavbar
            }).ToList();

            var configuracionResult = new ConfigurationDto
            {
                UserId = configuracion.UserId,
                Sections = sectionsDto
            };

            return CreatedAtAction(nameof(GetByUserId), new { userId = configuracion.UserId }, configuracionResult);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { error = "No autorizado" });
            }
        }

        [HttpPost("{userId}/sections")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSection(string userId, [FromBody] Section section)
        {
            var success = await _service.AddSectionAsync(userId, section);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Update(string userId, ConfigurationDto dto)
        {
            var existing = await _service.GetByUserIdAsync(userId);
            if (existing is null) return NotFound();

            var userIdLogguer = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Obtener el ID del usuario del token
            if (string.IsNullOrEmpty(userIdLogguer))
            {
                return Unauthorized(new { error = "Usuario no autenticado" });
            }
            if(userIdLogguer != userId)
            {
                return Unauthorized(new { error = "estas editando la parametria de otro usuario" });
            }

            // Asignar el ID del usuario autenticado a la configuración
            dto.UserId = userIdLogguer;
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
                Content = s.Content?.Select(c => new Content // Fix: Ensure Content is mapped to a List<Content>
                {
                    Position = c.Position,
                    IsHidden = c.IsHidden,
                    Stuffed = c.Stuffed,
                    ComponenteIdentifier = c.componenteIdentifier,
                    type = c.type
                }).ToList(),
                IsNavbar = s.IsNavbar
            }).ToList();

            var configuracion = new Configuracion
            {
                UserId = dto.UserId,
                Sections = sections
            };

            configuracion.UserId = userId;
            configuracion.Id = existing.Id;
            await _service.UpdateAsync(userId, configuracion);
            return NoContent();
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> Delete(string userId)
        {
            var existing = await _service.GetByUserIdAsync(userId);
            if (existing is null) return NotFound();

            await _service.DeleteAsync(userId);
            return NoContent();
        }
    }
}