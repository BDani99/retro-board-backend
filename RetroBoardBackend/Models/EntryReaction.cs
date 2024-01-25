using RetroBoardBackend.Enums;

namespace RetroBoardBackend.Models
{
    public class EntryReaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EntryId { get; set; }
        public ReactionTypes ReactionTypes { get; set; }

        public User User { get; set; } = null!;
        public Entry Entry { get; set; } = null!;
    }
}
