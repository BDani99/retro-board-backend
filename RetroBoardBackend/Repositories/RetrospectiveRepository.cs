using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;

namespace RetroBoardBackend.Repositories
{
    public class RetrospectiveRepository : IRetrospectiveRepository
    {
        private readonly ApplicationDbContext _context;

        public RetrospectiveRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Entry>> GetAllEntriesByIdAsync(int id)
        {
            var retrospective = await _context.Retrospectives
                .Include(x => x.Entries)
                .ThenInclude(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (retrospective is null)
            {
                var emptyCollection = new List<Entry>();
                return emptyCollection;
            }

            var entries = retrospective.Entries;

            return entries;
        }

        public Task<Retrospective?> GetByIdAsync(int id)
        {
            return _context.Retrospectives.Include(x => x.Entries).ThenInclude(x => x.EntryReactions).FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<Retrospective> CreateAsync(Retrospective retrospective)
        {
            _context.Retrospectives.Add(retrospective);
            await _context.SaveChangesAsync();
            return retrospective;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
