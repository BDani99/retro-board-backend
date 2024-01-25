using RetroBoardBackend.Dtos.Responses;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryResponse>> GetAllAsync();
        Task<CategoryResponse> GetByIdAsync(int id);
    }
}