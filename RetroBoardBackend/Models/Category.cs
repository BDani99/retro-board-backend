namespace RetroBoardBackend.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<Entry> Entries { get; set; } = new List<Entry>();
    }
}
