using RetroBoardBackend.Enums;

namespace RetroBoardBackend.Models
{
    public class Entry
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int AssigneeId { get; set; }
        public int RetrospectiveId { get; set; }
        public string EntryContent { get; set; } = string.Empty;
        public ColumnTypes ColumnType { get; set; }

        public User Author { get; set; } = null!;
        public User Assignee { get; set; } = null!;
        public Retrospective Retrospective { get; set; } = null!;
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<EntryReaction> EntryReactions { get; set; } = new List<EntryReaction>();
    }
}
