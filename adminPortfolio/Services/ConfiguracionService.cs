using adminportfolio.Models;
using adminProfolio.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace adminportfolio.Services
{
    public class ConfiguracionService
    {
        private readonly IMongoCollection<Configuracion> _configuraciones;
        public ConfiguracionService(IMongoDatabase database)
        {
         _configuraciones = database.GetCollection<Configuracion>("Configuracion");
        }

        public async Task<List<Configuracion>> GetAsync() =>
            await _configuraciones.Find(_ => true).ToListAsync();

        public async Task<Configuracion?> GetByUserIdAsync(string userId) =>
            await _configuraciones.Find(c => c.UserId == userId).FirstOrDefaultAsync();

        public async Task CreateAsync(Configuracion configuracion)
        {
            await _configuraciones.InsertOneAsync(configuracion);
        }

        public async Task UpdateAsync(string userId, Configuracion configuracion) =>
            await _configuraciones.ReplaceOneAsync(c => c.UserId == userId, configuracion);

        public async Task DeleteAsync(string userId) =>
            await _configuraciones.DeleteOneAsync(c => c.UserId == userId);

        public async Task<bool> AddSectionAsync(string configId, Section section)
        {
            var update = Builders<Configuracion>.Update.Push(c => c.Sections, section);
            var result = await _configuraciones.UpdateOneAsync(
                c => c.Id == configId,
                update
            );
            return result.ModifiedCount > 0;
        }
    }
}