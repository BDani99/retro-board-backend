using Microsoft.AspNetCore.Identity;

namespace RetroBoardBackend.Models
{
    public class User : IdentityUser<int>
    {
        public string Image { get; set; } = string.Empty;
        public ICollection<Project> ProjectsWhereAuthor { get; set; } = new List<Project>();
        public ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        public ICollection<EntryReaction> EntryReactions { get; set; } = new List<EntryReaction>();
        public ICollection<Entry> EntriesWhereAuthor { get; set; } = new List<Entry>();
        public ICollection<Entry> EntriesWhereAssignee { get; set; } = new List<Entry>();
    }
}
