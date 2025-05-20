using adminProfolio.Dtos;
using adminProfolio.Models;
using adminProfolio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace adminProfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Protege todas las rutas del controlador
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var usuarios = await _usuarioService.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [AllowAnonymous] // se deja sin la autenticación ya que es necesario para le registro del usuario
        [HttpPost]
        public async Task<IActionResult> CrearAsync(CreateUserDto usuario)
        {
            try
            {
                var existe = await _usuarioService.ObtenerPorEmailAsync(usuario.email);
                if (existe != null)
                {
                    return Conflict("Email ya registrado.");
                }
                await _usuarioService.CrearAsync(usuario);
                return CreatedAtAction(nameof(ObtenerTodos), new { id = usuario.Id }, usuario);
            }
            catch (ArgumentException ex)
            {
                // Error de validación de contraseña o email
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous] // se deja sin la autenticación ya que es necesario para le registro del usuario
        [HttpPost("verificar-email")]
        public async Task<IActionResult> VerificarEmail([FromBody] VerifyEmailDto dto)
        {
            try
            {
                var usuario = await _usuarioService.ObtenerPorEmailAsync(dto.email);

                if (usuario == null)
                {
                    return NotFound(new { error = "Usuario no encontrado" });
                }

                if (usuario.IsVerified)
                {
                    return Ok(new { message = "El correo ya ha sido verificado" });
                }

                if (usuario.VerificationCode != dto.codigo)
                {
                    return BadRequest(new { error = "Código de verificación inválido" });
                }

                if (usuario.VerificationCodeExpires.HasValue && DateTime.UtcNow > usuario.VerificationCodeExpires.Value)
                {
                    return BadRequest(new { error = "El código de verificación ha expirado" });
                }
                var resultado = await _usuarioService.VerificarEmailAsync(dto.email, dto.codigo, usuario);    
                return Ok(new { message = resultado });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Ocurrió un error interno.", detalles = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(string id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var usuarioActualizado = await _usuarioService.ActualizarAsync(id, dto);
                if (usuarioActualizado == null)
                    return NotFound(new { error = $"Usuario con ID \"{id}\" no encontrado." });

                return Ok(usuarioActualizado);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Error de validación de contraseña o email
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            var eliminado = await _usuarioService.EliminarAsync(id);
            if (!eliminado)
            {
                return NotFound(new { error = $"Usuario con ID \"{id}\" no encontrado." });
            }

            return NoContent();
        }

        [HttpPost("{id}/cambiar-password")]
        public async Task<IActionResult> CambiarPassword(string id, [FromBody] ChangePasswordDto dto)
        {
            try
            {
                await _usuarioService.CambiarPasswordAsync(id, dto);
                return Ok(new { message = "Contraseña actualizada correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                // Error de validación de contraseña insegura
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous] // se deja sin la autenticación ya que es necesario para le registro del usuario
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var usuario = await _usuarioService.LoginAsync(dto);
                return Ok(usuario);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}

