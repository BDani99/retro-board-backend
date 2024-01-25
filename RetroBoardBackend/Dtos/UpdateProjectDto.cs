namespace RetroBoardBackend.Dtos
{
    public class UpdateProjectDto
    {
        public string Image { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool HasRetroboard { get; set; }
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<int> UserIds { get; set; } = new List<int>();
    }
}