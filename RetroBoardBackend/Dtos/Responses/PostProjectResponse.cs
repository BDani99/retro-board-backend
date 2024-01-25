namespace RetroBoardBackend.Dtos.Responses
{
    public class PostProjectResponse
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ICollection<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();
        public ICollection<UserResponse> Users { get; set; } = new List<UserResponse>();

        public override bool Equals(object? obj)
        {
            return obj is PostProjectResponse response &&
                   Id == response.Id &&
                   Image == response.Image &&
                   Name == response.Name &&
                   Description == response.Description &&
                   Categories.Count == response.Categories.Count &&
                   Users.Count == response.Users.Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Image, Name, Description);
        }
    }
}