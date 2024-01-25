using RetroBoardBackend.Models;

namespace RetroBoardBackend.Repositories.Interfaces
{
    public interface IEntryReactionReposiotry
    {
        Task<EntryReaction> CreateAsync(EntryReaction entryReaction);
        Task<bool> AlreadyReacted(int userId, int entryId);
        Task<bool> DeleteAsync(int userId, int entryId);
    }
}