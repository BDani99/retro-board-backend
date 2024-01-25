namespace RetroBoardBackend.Dtos
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string? Image { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}