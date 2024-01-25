using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface IEntryReactionService
    {
        Task<EntryReactionResponse> CreateAsync(EntryReactionDto entryReactionDto);
        Task<bool> AlreadyReacted(int userId, int entryId);
        Task<bool> DeleteAsync(int userId, int entryId);
    }
}