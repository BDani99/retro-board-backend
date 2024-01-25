using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroBoardBackend.Dtos;
using RetroBoardBackend.Dtos.Responses;
using RetroBoardBackend.Services.Interfaces;

namespace RetroBoardBackend.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly IEntryService _entryService;

        public EntriesController(IEntryService entryService)
        {
            _entryService = entryService;
        }

        [HttpGet("{entry_id}/categories")]
        public async Task<ActionResult<List<CategoryResponse>>> GetAllCategoriesByIdAsync(int entry_id)
        {
            var categories = await _entryService.GetAllCategoriesByIdAsync(entry_id);
            return Ok(categories);
        }

        [HttpPost]
        public async Task<ActionResult<EntryResponse>> CreateAsync(EntryDto entryDto)
        {
            var newEntry = await _entryService.CreateAsync(entryDto);
            return Ok(newEntry);
        }
    }
}
