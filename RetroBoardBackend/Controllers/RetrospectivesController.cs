using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroBoardBackend.Constans;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class RetrospectivesController : ControllerBase
    {
        private readonly IRetrospectiveService _retrospectiveService;
        private readonly IProjectService _projectService;

        public RetrospectivesController(IRetrospectiveService retrospectiveService, 
            IProjectService projectService)
        {
            _retrospectiveService = retrospectiveService;
            _projectService = projectService;
        }

        [HttpGet("{retro_id}/entries")]
        public async Task<ActionResult<ICollection<EntryResponse>>> GetAllEntriesByIdAsync(int retro_id)
        {
            var retrospective = await _retrospectiveService.GetByIdAsync(retro_id);

            if (retrospective == null)
            {
                return BadRequest(new { error = ErrorMessages.RETROSPECTIVE_NOT_FOUND });
            }

            var entries = await _retrospectiveService.GetAllEntriesByIdAsync(retro_id);
            return Ok(entries);
        }

        [HttpPost]
        public async Task<ActionResult<RetrospectiveResponse>> CreateAsync(RetrospectiveDto retrospectiveDto)
        {
            var project = await _projectService.GetProjectByIdAsync(retrospectiveDto.ProjectId);
            
            if (project == null)
            {
                return BadRequest(new { error = ErrorMessages.PROJECT_NOT_FOUND });
            }

            var newRetrospective = await _retrospectiveService.CreateAsync(retrospectiveDto);

            return Ok(newRetrospective);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, UpdateRetrospectiveDto updateRetrospectiveDto)
        {
            var retrospective = await _retrospectiveService.GetByIdAsync(id);

            if (retrospective == null)
            {
                return NotFound(new { error = ErrorMessages.RETROSPECTIVE_NOT_FOUND });
            }

            await _retrospectiveService.UpdateAsync(id, updateRetrospectiveDto);

            return NoContent();
        }
    }
}
