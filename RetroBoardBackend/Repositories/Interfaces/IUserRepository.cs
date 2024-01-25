using RetroBoardBackend.Models;

namespace RetroBoardBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<UserRole?> GetUserRoleByIdAsync(int id);
        Task<int> SaveChangesAsync();
    }
}