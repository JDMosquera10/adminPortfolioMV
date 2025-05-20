namespace adminportfolio.Dtos
{
    public class ComponenteDto
    {
        public string? Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Component { get; set; }
        public required string ComponentIdentifier { get; set; }
        public required string ComponentType { get; set; }
        public required string ComponentVersion { get; set; }
        public object? Data { get; set; }
        public string? LoadDataAction { get; set; }
        public List<string>? Actions { get; set; }
    }
}