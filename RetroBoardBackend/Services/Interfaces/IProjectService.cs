using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;

namespace RetroBoardBackend.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResult> CreateAsync(ProjectDto projectDto);
        Task<ProjectResult> UpdateAsync(int id, UpdateProjectDto updateProjectDto);
        Task<ProjectResult> DeleteAsync(int id);
        Task<ProjectResult> GetProjectByIdAsync(int id);
        Task<ProjectResult> GetAllProjectAsync();
        Task<List<RetrospectiveResponseWithStats>> GetAllRetrospectivesByIdAsync(int id);
    }
}