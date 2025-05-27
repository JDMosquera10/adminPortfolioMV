using adminportfolio.Models;
using System.Text.Json;
using MongoDB.Bson;

namespace adminportfolio.Dtos
{
    public class SectionsDto
    {
        public int Order { get; set; }
        public string fullname { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? Componente_identifier { get; set; } = string.Empty;
        public bool? RenderClient { get; set; } = false;
        public List<ContentDto>? Content { get; set; } = null;
        public bool? IsNavbar { get; set; } = false;

    }

    public class ContentDto
    {
        public string Position { get; set; } = string.Empty;
        public bool? IsHidden { get; set; } = false;
        public bool? Stuffed { get; set; } = false;
        public string componenteIdentifier { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
    }
}
