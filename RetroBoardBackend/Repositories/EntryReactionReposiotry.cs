using Microsoft.EntityFrameworkCore;
using RetroBoardBackend.Data;
using RetroBoardBackend.Models;
using RetroBoardBackend.Repositories.Interfaces;

namespace RetroBoardBackend.Repositories
{
    public class EntryReactionReposiotry : IEntryReactionReposiotry
    {
        private readonly ApplicationDbContext _context;

        public EntryReactionReposiotry(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EntryReaction> CreateAsync(EntryReaction entryReaction)
        {
            _context.EntryReactions.Add(entryReaction);
            await _context.SaveChangesAsync();
            return entryReaction;
        }

        public async Task<bool> AlreadyReacted(int userId, int entryId)
        {
            var reaction = await _context.EntryReactions
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.EntryId.Equals(entryId));

            return reaction == null ? false : true;
        }

        public async Task<bool> DeleteAsync(int userId, int entryId)
        {
            var reaction = await _context.EntryReactions
                .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.EntryId.Equals(entryId));

            if (reaction == null)
            {
                return false;
            }

            _context.EntryReactions.Remove(reaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
