using MongoDB.Driver;
using adminProfolio.Models;
using Org.BouncyCastle.Crypto.Generators;
using adminProfolio.Dtos;
using adminProfolio.Interfaces;

namespace adminProfolio.Services
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _usuarios;
        private readonly EmailService _emailService;
        private readonly ITokenService _tokenService;

        public UsuarioService(IMongoDatabase database, EmailService emailServices, ITokenService tokenService)
        {
            _usuarios = database.GetCollection<Usuario>("usuarios");
            _emailService = emailServices;
            _tokenService = tokenService;
        }

        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            return await _usuarios.Find(_ => true).ToListAsync();
        }

        public async Task<Usuario?> ObtenerPorIdAsync(string id)
        {
            return await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            return await _usuarios.Find(u => u.email == email.ToLower()).FirstOrDefaultAsync();
        }

        public async Task<Usuario> CrearAsync(CreateUserDto parameters)
        {

            Usuario usuario = new Usuario(parameters);
            usuario.email = usuario.email.ToLower();
            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);
            var codigoVerificacion = new Random().Next(100000, 999999).ToString();
            usuario.VerificationCode = codigoVerificacion;
            usuario.VerificationCodeExpires = DateTime.UtcNow.AddMinutes(5);

            await _usuarios.InsertOneAsync(usuario);

            await _emailService.EnviarCodigoVerificacion(usuario.email, usuario.fullname, codigoVerificacion);

            usuario.password = null!;
            usuario.VerificationCode = null!;
            usuario.VerificationCodeExpires = null;

            return usuario;
        }

        public async Task<string> VerificarEmailAsync(string email, string codigo, Usuario usuario)
        {
            var update = Builders<Usuario>.Update
                .Set(u => u.IsVerified, true)
                .Unset(u => u.VerificationCode)
                .Unset(u => u.VerificationCodeExpires);

            await _usuarios.UpdateOneAsync(u => u.Id == usuario.Id, update);

            return "Correo verificado correctamente.";
        }

        public async Task<Usuario?> ActualizarAsync(string id, UpdateUserDto dto)
        {
            var usuario = await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
            dto.email.ToLower();
            if (usuario == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.email) && dto.email != usuario.email)
            {
                var emailExistente = await _usuarios.Find(u => u.email == dto.email).FirstOrDefaultAsync();
                if (emailExistente != null)
                {
                    throw new InvalidOperationException("El correo electrónico ya está registrado.");
                }

                usuario.email = dto.email;
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                usuario.password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            if (!string.IsNullOrEmpty(dto.fullname)) usuario.fullname = dto.fullname;
            if (!string.IsNullOrEmpty(dto.email)) usuario.email = dto.email;
            if (dto.phone_number.HasValue) usuario.phone_number = dto.phone_number.Value;

            await _usuarios.ReplaceOneAsync(u => u.Id == id, usuario);

            usuario.password = null!;
            return usuario;
        }

        public async Task<bool> EliminarAsync(string id)
        {
            var resultado = await _usuarios.DeleteOneAsync(u => u.Id == id);
            return resultado.DeletedCount > 0;
        }

        public async Task CambiarPasswordAsync(string id, ChangePasswordDto dto)
        {
            var usuario = await _usuarios.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuario con ID \"{id}\" no encontrado.");
            }

            bool passwordValido = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, usuario.password);
            if (!passwordValido)
            {
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");
            }

            usuario.password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password);
            var codigoVerificacion = new Random().Next(100000, 999999).ToString();
            usuario.VerificationCode = codigoVerificacion;
            usuario.VerificationCodeExpires = DateTime.UtcNow.AddMinutes(5);

            await _emailService.EnviarCodigoVerificacion(usuario.email, usuario.fullname, codigoVerificacion);

            await _usuarios.ReplaceOneAsync(u => u.Id == id, usuario);
        }

        public async Task<Usuario> LoginAsync(LoginDto dto)
        {
            var user = await _usuarios.Find(u => u.email == dto.email.ToLower()).FirstOrDefaultAsync();

            if (user == null)
                throw new UnauthorizedAccessException("Credenciales inválidas");

            if (!user.IsVerified)
                throw new UnauthorizedAccessException("Por favor verifica tu correo electrónico antes de iniciar sesión");

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.password, user.password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Credenciales inválidas");

            var role = user.role ?? "User";

            var (accessToken, refreshToken) = await _tokenService.GetTokensAsync(user.Id, user.email, role);

            var codigoVerificacion = new Random().Next(100000, 999999).ToString();
            user.VerificationCode = codigoVerificacion;
            user.VerificationCodeExpires = DateTime.UtcNow.AddMinutes(2);
            user.token = accessToken;
            user.refresh_token = refreshToken;

            var filter = Builders<Usuario>.Filter.Eq(u => u.Id, user.Id);
            await _usuarios.ReplaceOneAsync(filter, user);

            user.password = null!;
            user.VerificationCode = null!;
            user.VerificationCodeExpires = null;

            return user;
        }

    }
}
