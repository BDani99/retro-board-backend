namespace RetroBoardBackend.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int PmId { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool HasRetroboard { get; set; }
        public bool IsDeleted { get; set; }
        public User AuthorUser { get; set; } = null!;
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Retrospective> Retrospectives { get; set; } = new List<Retrospective>();
    }
}
