using RetroBoardBackend.Models;

namespace RetroBoardBackend.Repositories.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> CreateAsync(Project project);
        Task<List<Project>> GetAllAsync();
        Task SaveChangesAsync();
        Task<Project?> GetByIdAsync(int id);
        Task<ICollection<Retrospective>> GetAllRetrospectivesByIdAsync(int id);
        Task<List<Project>> GetProjectsWhereAuthorWithUserIdAsync(int userId);
    }
}