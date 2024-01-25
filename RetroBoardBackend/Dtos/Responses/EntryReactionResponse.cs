namespace RetroBoardBackend.Dtos.Responses
{
    public class EntryReactionResponse
    {
        public int Id { get; set; }
        public UserResponse User { get; set; } = null!;
        public EntryResponse Entry { get; set; } = null!;
        public string ReactionType { get; set; } = null!;
    }
}
