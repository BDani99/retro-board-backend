namespace RetroBoardBackend.Dtos.Responses
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public UserResponse PmUser { get; set; } = null!;
        public string? Image { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool HasRetroboard { get; set; }
        public List<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();

        public override bool Equals(object? obj)
        {
            return obj is ProjectResponse response &&
                   Id == response.Id &&
                   Name == response.Name &&
                   Description == response.Description &&
                   PmUser.Id == response.PmUser.Id &&
                   Categories.Count == response.Categories.Count &&
                   HasRetroboard == response.HasRetroboard;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
