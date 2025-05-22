namespace adminportfolio.Dtos
{
    public class ConfigurationDto
    {
        public string UserId { get; set; } = string.Empty;

        public List<SectionsDto> Sections { get; set; } = new List<SectionsDto>();
    }
}
