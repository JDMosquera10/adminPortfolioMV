using System.Text.Json;

namespace adminportfolio.Dtos
{
    public class ComponenteDto
    {
        public string? Id { get; set; }
        public required string Type { get; set; }
        public required string Identifier { get; set; }
        public JsonElement? Props_json { get; set; }
    }
}