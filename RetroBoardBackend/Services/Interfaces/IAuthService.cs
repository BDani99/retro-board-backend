using RetroBoardBackend.Dtos;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto?> LoginAsync(LoginDto loginDto);
    }
}