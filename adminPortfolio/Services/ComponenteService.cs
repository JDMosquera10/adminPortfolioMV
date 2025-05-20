using adminportfolio.Models;
using MongoDB.Driver;

namespace adminportfolio.Services
{
    public class ComponenteService
    {
        private readonly IMongoCollection<Componente> _componentes;

        public ComponenteService(IMongoDatabase database)
        {
            _componentes = database.GetCollection<Componente>("Componentes");
        }

        public async Task<List<Componente>> GetAsync() =>
            await _componentes.Find(_ => true).ToListAsync();

        public async Task<Componente?> GetByIdAsync(string id) =>
            await _componentes.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Componente componente) =>
            await _componentes.InsertOneAsync(componente);

        public async Task UpdateAsync(string id, Componente componente) =>
            await _componentes.ReplaceOneAsync(c => c.Id == id, componente);

        public async Task DeleteAsync(string id) =>
            await _componentes.DeleteOneAsync(c => c.Id == id);
    }
}