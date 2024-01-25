using RetroBoardBackend.Models;

namespace RetroBoardBackend.Dtos.Responses
{
    public class EntryResponse
    {
        public int Id { get; set; }
        public string EntryContent { get; set; } = string.Empty;
        public ICollection<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();
        public RetrospectiveResponse Retrospective { get; set; } = null!;
        public UserResponse Assignee { get; set; } = null!;
        public UserResponse Author { get; set; } = null!;
        public string ColumnType { get; set; } = string.Empty;
        public ReactionAmount ReactionAmount { get; set; } = null!;
        public string? CurrentUserReaction { get; set; }
    }
}
