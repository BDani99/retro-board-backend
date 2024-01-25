using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Dtos.Result;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("{project_id}/retrospectives")]
        public async Task<ActionResult<List<RetrospectiveResponseWithStats>>> GetAllRetrospectivesByIdAsync(int project_id)
        {
            var retrospectives = await _projectService.GetAllRetrospectivesByIdAsync(project_id);
            return Ok(retrospectives);
        }

        [HttpPost]
        public async Task<ActionResult<PostProjectResponse>> CreateAsync(ProjectDto projectDto)
        {
            var projectResult = await _projectService.CreateAsync(projectDto);
            if (projectResult.ErrorMessage != null)
            {
                return BadRequest(new { Error = projectResult.ErrorMessage });
            }

            return Ok(projectResult.ProjectResponses);
        }

        [HttpPatch]
        public async Task<ActionResult> UpdateAsync(int id, UpdateProjectDto updateProjectDto)
        {
            var projectResult = await _projectService.UpdateAsync(id, updateProjectDto);
            if (projectResult.ErrorMessage != null)
            {
                return BadRequest(new { Error = projectResult.ErrorMessage });
            }

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var projectResult = await _projectService.DeleteAsync(id);
            if (projectResult.ErrorMessage != null)
            {
                return BadRequest(new { Error = projectResult.ErrorMessage });
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetByIdAsync(int id)
        {
            var projectResult = await _projectService.GetProjectByIdAsync(id);
            if (projectResult.ErrorMessage != null)
            {
                return NotFound(new { Error = projectResult.ErrorMessage });
            }

            return Ok(projectResult.ProjectResponse);
        }

        [HttpGet]
        public async Task<ActionResult<ProjectResult>> GetAllAsync()
        {
            var projectResult = await _projectService.GetAllProjectAsync();
            if (projectResult.ErrorMessage != null)
            {
                return BadRequest(new { Error = projectResult.ErrorMessage });
            }

            return Ok(projectResult.ProjectResponses);
        }
    }
}