namespace RetroBoardBackend.Models
{
    public class Retrospective
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool StatsNeeded { get; set; }
        public bool IsActive { get; set; }

        public Project Project { get; set; } = null!;
        public ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}
