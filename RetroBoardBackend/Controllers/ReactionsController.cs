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
    public class ReactionsController : ControllerBase
    {
        private readonly IEntryReactionService _reactionService;
        private readonly IEntryService _entryService;
        private readonly IUserService _userService;

        public ReactionsController(IEntryReactionService reactionService,
            IEntryService entryService,
            IUserService userService)
        {
            _reactionService = reactionService;
            _entryService = entryService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<EntryReactionResponse>> CreateAsync(EntryReactionDto entryReactionDto)
        {
            var me = await _userService.GetMyselfAsync();
            var entry = await _entryService.GetByIdAsync(entryReactionDto.EntryId);
            var alreadyReacted = await _reactionService.AlreadyReacted(me.Id, entryReactionDto.EntryId);

            if (entry == null)
            {
                return NotFound(new { error = ErrorMessages.ENTRY_NOT_FOUND });
            }

            if (!alreadyReacted)
            {
                var reaction = await _reactionService.CreateAsync(entryReactionDto);

                return Ok(reaction);
            }

            await _reactionService.DeleteAsync(me.Id, entryReactionDto.EntryId);

            var newReaction = await _reactionService.CreateAsync(entryReactionDto);

            return Ok(newReaction);
        }
    }
}
