using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResult> GetAllAsync();
        Task<UserResponse?> GetByIdAsync(int id);
        Task<UserResponse?> GetMyselfAsync();
        Task<UserResult> RegisterAsync(RegisterDto registerDto);
        Task UpdateAsync(int id, UpdateUserDto updateUserDto);
        Task<IEnumerable<MyProjectResponse>?> GetAllMyProjectsAsync();
    }
}