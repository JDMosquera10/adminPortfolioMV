using adminportfolio.Models;
using adminProfolio.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace adminportfolio.Services
{
    public class ConfiguracionService
    {
        private readonly IMongoCollection<Componente> _componentes;
        private readonly IMongoCollection<Configuracion> _configuraciones;
        public ConfiguracionService(IMongoDatabase database)
        {
         _configuraciones = database.GetCollection<Configuracion>("Configuracion");
            _componentes = database.GetCollection<Componente>("Componentes");
        }

        public async Task<List<Configuracion>> GetAsync() =>
            await _configuraciones.Find(_ => true).ToListAsync();

        public async Task<Configuracion?> GetByUserIdAsync(string userId) =>
            await _configuraciones.Find(c => c.UserId == userId).FirstOrDefaultAsync();


        public async Task<Configuracion?> GetConfigDataWithComponentes(string userId)
        {
            var config = await _configuraciones.Find(c => c.UserId == userId).FirstOrDefaultAsync();
            if (config == null) return null;

            foreach (var section in config.Sections)
            {
                if (section.Type == "componente" && !string.IsNullOrEmpty(section.Componente_identifier))
                {
                    // Busca el componente por Componente_identifier
                    var componente = await _componentes.Find(c => c.Identifier == section.Componente_identifier).FirstOrDefaultAsync();
                    if (componente != null)
                    {
                        section.ContentData = componente.ToBsonDocument();
                    }
                }
                else if (section.Type == "panel" && section.Content != null)
                {
                    foreach (var contentItem in section.Content)
                    {
                        if (!string.IsNullOrEmpty(contentItem.ComponenteIdentifier))
                        {
                            var componente = await _componentes.Find(c => c.Identifier == contentItem.ComponenteIdentifier).FirstOrDefaultAsync();
                            if (componente != null)
                            {
                                contentItem.ContentData = componente.ToBsonDocument(); // Convertir Componente a BsonDocument
                            }
                        }
                    }
                }
            }

            return config;
        }

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