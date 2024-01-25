using RetroBoardBackend.Models;

namespace RetroBoardBackend.Repositories.Interfaces
{
    public interface IRetrospectiveRepository
    {
        Task<Retrospective> CreateAsync(Retrospective retrospective);
        Task<int> SaveChangesAsync();
        Task<Retrospective?> GetByIdAsync(int id);
        Task<ICollection<Entry>> GetAllEntriesByIdAsync(int id);
    }
}