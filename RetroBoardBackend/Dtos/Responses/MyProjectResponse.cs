namespace RetroBoardBackend.Dtos.Responses
{
    public class MyProjectResponse
    {
        public int Id { get; set; }
        public UserResponse AuthorUser { get; set; } = null!;
        public string Image { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ICollection<CategoryResponse> Categories { get; set; } = new List<CategoryResponse>();
        public int UserCount { get; set; }

    }
}
