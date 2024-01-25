using RetroBoardBackend.Models;

namespace RetroBoardBackend.Repositories.Interfaces
{
    public interface IEntryRepository
    {
        Task<Entry> CreateAsync(Entry entry);
        Task<Entry?> GetByIdAsync(int id);
        Task<ICollection<Category>> GetAllCategoriesByIdAsync(int id);
    }
}