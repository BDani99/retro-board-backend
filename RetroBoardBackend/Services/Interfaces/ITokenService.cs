using RetroBoardBackend.Dtos;
using RetroBoardBackend.Models;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> GenerateJwtTokenAsync(User user);
        int GetMyId();
    }
}