namespace RetroBoardBackend.Dtos
{
    public class RetrospectiveDto
    {
        public string Name { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public bool StatsNeeded { get; set; }
    }
}
