using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;

namespace RetroBoardBackend.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Category>> GetAllAsync()
        {
            return _context.Categories.ToListAsync();
        }

        public Task<Category?> GetByIdAsync(int id)
        {
            return _context.Categories.FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public Task<Category?> GetByNameAsync(string name)
        {
            return _context.Categories.FirstOrDefaultAsync(c => c.Name.Equals(name));
        }
    }
}