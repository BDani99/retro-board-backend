namespace RetroBoardBackend.Dtos.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Image { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
