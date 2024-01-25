using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;

namespace RetroBoardBackend.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Entry?> GetByIdAsync(int id)
        {
            return _context.Entries
                .Include(x => x.Categories)
                .Include(x => x.Retrospective)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<ICollection<Category>> GetAllCategoriesByIdAsync(int id)
        {
            var entry = await _context.Entries
                .Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (entry == null)
            {
                return new List<Category>();
            }

            return entry.Categories;
        }

        public async Task<Entry> CreateAsync(Entry entry)
        {
            _context.Entries.Add(entry);
            await _context.SaveChangesAsync();
            return entry;
        }
    }
}
