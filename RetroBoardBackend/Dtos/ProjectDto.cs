namespace RetroBoardBackend.Dtos
{
    public class ProjectDto
    {
        public string? Image { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> UserIds { get; set; } = new List<int>();
    }
}
