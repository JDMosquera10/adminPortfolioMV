using System.Text.Json;

namespace adminportfolio.Dtos
{
    public class ConfiguracionGeneralDto
    {
        public string UserId { get; set; } = string.Empty;

        public List<SectionsDtoGeneral> Sections { get; set; } = new List<SectionsDtoGeneral>();
    }

    public class SectionsDtoGeneral
    {
        public int Order { get; set; }
        public string fullname { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Title { get; set; } = string.Empty;
        public string? Componente_identifier { get; set; } = string.Empty;

        public JsonElement? contentData { get; set; }
        public bool? RenderClient { get; set; } = false;
        public List<ContentDtoGeneral>? Contents { get; set; } = null;
        public bool? IsNavbar { get; set; } = false;
    }

    public class ContentDtoGeneral
    {
        public string Position { get; set; } = string.Empty;
        public bool? IsHidden { get; set; } = false;

        public JsonElement? contentData { get; set; }
        public bool? Stuffed { get; set; } = false;
        public string componenteIdentifier { get; set; } = string.Empty;
    }
}