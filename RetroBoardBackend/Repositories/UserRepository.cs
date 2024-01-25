using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;

namespace RetroBoardBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<User>> GetAllAsync()
        {
            return _context.Users.ToListAsync();
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users
            .Include(u => u.Projects)
                .ThenInclude(p => p.Users)
            .Include(u => u.Projects)
                .ThenInclude(p => p.Categories)
            .Include(u => u.Projects)
                .ThenInclude(p => p.AuthorUser)
            .SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public Task<UserRole?> GetUserRoleByIdAsync(int id)
        {
            return _context.UserRoles.Include(x => x.Role).FirstOrDefaultAsync(x => x.UserId == id);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
