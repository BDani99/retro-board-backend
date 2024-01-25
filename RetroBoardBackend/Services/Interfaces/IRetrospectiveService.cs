using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface IRetrospectiveService
    {
        Task<RetrospectiveResponse> CreateAsync(RetrospectiveDto retrospectiveDto);
        Task<RetrospectiveResponse> GetByIdAsync(int id);
        Task UpdateAsync(int id, UpdateRetrospectiveDto updateRetrospectiveDto);
        Task<List<EntryResponse>> GetAllEntriesByIdAsync(int id);
    }
}