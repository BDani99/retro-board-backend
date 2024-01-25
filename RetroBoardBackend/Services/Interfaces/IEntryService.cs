using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface IEntryService
    {
        Task<EntryResponse> CreateAsync(EntryDto entryDto);
        Task<EntryResponse?> GetByIdAsync(int id);
        Task<List<CategoryResponse>> GetAllCategoriesByIdAsync(int id);
    }
}